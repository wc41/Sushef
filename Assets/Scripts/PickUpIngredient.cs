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
        public Transform hitPoint;
        RaycastHit hit;

        Vector3 offset;

        private static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        [SerializeField]
        [Tooltip("Instantiates this prefab on a plane at the touch location.")]
        private GameObject placedPrefab;

        private ARRaycastManager m_RaycastManager;
        private bool pressed;

        private Ray lastRay;
        private GameObject g;


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
            this.m_RaycastManager = this.GetComponent<ARRaycastManager>();
        }
        private void Update()
        {
            // update after release
            if (Pointer.current != null && this.PickedUpObject != null && this.pressed == false)
            {

                // hit board
                if (Physics.Raycast(lastRay, out RaycastHit draghit, 1000, LayerMask.GetMask("Board")))
                {
                    dropOntoBoard();
                }

                // hit trash
                else if (Physics.Raycast(lastRay, out RaycastHit trashHit, 1000, LayerMask.GetMask("Trash"))) {
                    dropIntoTrash(trashHit);
                }

                this.PickedUpObject = null;
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

            if (Physics.Raycast(ray, out hit, 1000, LayerMask.GetMask("Game")))
            {                
                Debug.Log("$$$ I found: " + hit.transform.gameObject.name + ".");
                if (this.PickedUpObject == null)
                {
                    this.PickUpObject(hit);
                }
                this.lastRay = ray;
            }

            if (this.PickedUpObject != null)
            {
                this.UpdateObject(hit);
            }

        }

        private void PickUpObject(RaycastHit hit)
        {
            this.PickedUpObject = hit.collider.gameObject;

            g.GetPhotonView().RPC("TakeIngredientAway", RpcTarget.Others,
                this.PickedUpObject.GetComponent<PhotonView>().ViewID);

            this.PickedUpObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);

            hitPoint.transform.position = hit.point;
            offset = PickedUpObject.transform.position - hit.point;
        }

        private void UpdateObject(RaycastHit hit)
        {
            this.PickedUpObject.transform.position = hitPoint.transform.position + offset;
            //this.PickedUpObject.transform.position = hit.point;
        }

        private void dropOntoBoard()
        {
            Debug.Log("$$$ released on board");
            g = GameObject.FindGameObjectWithTag("GameManager");

            Vector3 puObjectPosition = this.PickedUpObject.transform.position;

            if (puObjectPosition.z > 0)
            {
                g.GetPhotonView().RPC("AddIngredientGlobal", RpcTarget.Others,
                this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1);
            }
            else if (puObjectPosition.z < 0)
            {
                g.GetPhotonView().RPC("AddIngredientGlobal", RpcTarget.Others,
                this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2);
            }
        }
        private void dropIntoTrash(RaycastHit trashHit)
        {
            Debug.Log("$$$ released on trash");
            g = GameObject.FindGameObjectWithTag("GameManager");

            Vector3 trashPos = trashHit.transform.position;
            if (trashPos.z > 0)
            {
                g.GetPhotonView().RPC("Trash", RpcTarget.Others,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 1);
            }
            else if (trashPos.z < 0)
            {
                g.GetPhotonView().RPC("Trash", RpcTarget.Others,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID, 2);
            }
        }

        protected override void OnPress(Vector3 position)
        {
            this.pressed = true;
        }

        protected override void OnPressCancel()
        {
            this.pressed = false;
            //this.PickedUpObject = null;
        }
    }
}
