namespace MyFirstARGame
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.XR.ARFoundation;
    using UnityEngine.XR.ARSubsystems;
    using Photon.Pun;

    /// <summary>
    /// Listens for touch events and performs an AR raycast from the screen touch point.
    /// AR raycasts will only hit detected trackables like feature points and planes.
    ///
    /// If a raycast hits a trackable, the <see cref="placedPrefab"/> is instantiated
    /// and moved to the hit position.
    /// </summary>
    [RequireComponent(typeof(ARRaycastManager))]
    public class PickUpIngredient : PressInputBase
    {
        RaycastHit hit;
        RaycastHit lastHit;

        float makeTime;

        public Vector3 hitPoint;
        Vector3 offset;

        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject placedPrefab;

        private ARRaycastManager m_RaycastManager;
        private bool pressed;

        private Ray lastRay;
        private GameObject g;

        private float hold;
        Time timer;


        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject PickedUpObject { get; private set; }

        GameObject ws;


        /// <summary>
        /// Gets or sets a value indicating whether the user is allowed to place an object.
        /// </summary>
        public bool CanPlace { get; set; }

        protected override void Awake()
        {
            base.Awake();
            makeTime = 1.5f;
            this.m_RaycastManager = this.GetComponent<ARRaycastManager>();
        }

        private void releasedOnBoard()
        {
            Debug.Log("$$$ released on board");
            g = GameObject.FindGameObjectWithTag("GameManager");

            if (this.PickedUpObject.tag == "Ingredient")
            {
                Vector3 puObjectPosition = this.PickedUpObject.transform.position;

                if (puObjectPosition.z > 0)
                {
                    g.GetPhotonView().RPC("AddIngredientGlobal", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else if (puObjectPosition.z < 0)
                {
                    g.GetPhotonView().RPC("AddIngredientGlobal", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                this.PickedUpObject = null;
            } else if (this.PickedUpObject.tag == "Wasabi")
            {
                makeTime *= 0.75f;
                Vector3 puObjectPosition = this.PickedUpObject.transform.position;

                if (puObjectPosition.z > 0)
                {
                    g.GetPhotonView().RPC("TrashWasabiGlobal", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else if (puObjectPosition.z < 0)
                {
                    g.GetPhotonView().RPC("TrashWasabiGlobal", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                this.PickedUpObject = null;
            }
        }

        private void releasedOnTrash(RaycastHit trashHit)
        {
            Debug.Log("$$$ released on trash");
            g = GameObject.FindGameObjectWithTag("GameManager");

            Vector3 trashPos = trashHit.transform.position;
            if (trashPos.z > 0)
            {
                g.GetPhotonView().RPC("Trash", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            else if (trashPos.z < 0)
            {
                g.GetPhotonView().RPC("Trash", RpcTarget.MasterClient,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2, PhotonNetwork.LocalPlayer.ActorNumber);
            }
            this.PickedUpObject = null;
        }

        private void releasedOnPlate(RaycastHit plateHit)
        {
            Debug.Log("$$$ released on plate");
            g = GameObject.FindGameObjectWithTag("GameManager");

            Vector3 platePos = plateHit.transform.position;
            if (this.PickedUpObject.tag == "Sushi")
            {
                if (platePos.z > 0)
                {
                    g.GetPhotonView().RPC("Sushi", RpcTarget.MasterClient,
                        this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                else if (platePos.z < 0)
                {
                    g.GetPhotonView().RPC("Sushi", RpcTarget.MasterClient,
                        this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2, PhotonNetwork.LocalPlayer.ActorNumber);
                }
                this.PickedUpObject = null;
            }
        }


        private void Update()
        {
            
            // update after release
            if (Pointer.current != null && this.PickedUpObject != null && this.pressed == false)
            {

                // hit board
                if (Physics.Raycast(lastRay, out RaycastHit draghit, 1000, LayerMask.GetMask("Board")))
                {
                    releasedOnBoard();
                }

                // hit trash
                else if (Physics.Raycast(lastRay, out RaycastHit trashHit, 1000, LayerMask.GetMask("Trash"))) {
                    releasedOnTrash(trashHit);
                }

                // hit plate
                else if (Physics.Raycast(lastRay, out RaycastHit plateHit, 1000, LayerMask.GetMask("Plate")))
                {
                    releasedOnPlate(plateHit);
                }
            }

            

            if (Pointer.current == null || this.pressed == false || !this.CanPlace)
                return;

            var touchPosition = Pointer.current.position.ReadValue();

            var ray = Camera.main.ScreenPointToRay(touchPosition);

            // Ensure we are not over any UI element.
            var uiButtons = FindObjectOfType<UIButtons>();
            if (uiButtons != null && (uiButtons.IsPointOverUI(touchPosition)))
                return;

            // Raycast against layer "GroundPlane" using normal Raycasting for our artifical ground plane.
            // For AR Foundation planes (if enabled), we use AR Raycasting.

            g = GameObject.FindGameObjectWithTag("GameManager");

            if (Physics.Raycast(ray, out RaycastHit recipeHit, 1000, LayerMask.GetMask("Recipe")))
            {
                Debug.Log("$$$ Opening recipe book");
                PhotonView.Find(1).gameObject.SetActive(true);
                this.CanPlace = false;
                return;
            }

            if (this.PickedUpObject == null)
            {
                if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Game")))
                {
                    Debug.Log("$$$ I found: " + hit.transform.gameObject.name + ".");

                    PickUpObject(hit);
                } else if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Sushi")))
                {
                    Debug.Log("$$$ I found: " + hit.transform.gameObject.name + ".");

                    PickUpObject(hit);
                } else if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Board")))
                {
                    hold += Time.deltaTime;
                    if (hold >= makeTime)
                    {
                        Vector3 puObjectPosition = hit.transform.position;

                        if (puObjectPosition.z > 0)
                        {
                            g.GetPhotonView().RPC("HoldToCreate", RpcTarget.MasterClient, 1, PhotonNetwork.LocalPlayer.ActorNumber);
                        }
                        else if (puObjectPosition.z < 0)
                        {
                            g.GetPhotonView().RPC("HoldToCreate", RpcTarget.MasterClient, 2, PhotonNetwork.LocalPlayer.ActorNumber);
                        }
                        hold = 0f;
                    }
                }
            }

            if (this.PickedUpObject != null)
            {
                if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("PlaneTracker")))
                {
                    this.lastRay = ray;
                    this.lastHit = hit;
                }
                this.lastRay = ray;
                UpdateObject(this.lastHit);
            }
        }

        private void PickUpObject(RaycastHit hit)
        {
            this.PickedUpObject = hit.transform.gameObject;

            g.GetPhotonView().RPC("TakeIngredientAway", RpcTarget.Others,
                this.PickedUpObject.GetComponent<PhotonView>().ViewID);

            this.PickedUpObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);

        }

        private void UpdateObject(RaycastHit hit)
        {
            if (this.PickedUpObject != null)
            {

                this.PickedUpObject.transform.position = hit.point;
            }
        }

        public void Restart()
        {
            g.GetPhotonView().RPC("CallPlaceOrder", RpcTarget.MasterClient);
        }


        protected override void OnPress(Vector3 position)
        {
            this.pressed = true;
        }

        protected override void OnPressCancel()
        {
            this.pressed = false;
        }
    }
}
