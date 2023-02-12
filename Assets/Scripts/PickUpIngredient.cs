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
            if (Pointer.current != null && this.PickedUpObject != null && this.pressed == false)
            {
                Debug.Log("$$$ released");
                if (Physics.Raycast(lastRay, out RaycastHit draghit, 1000, LayerMask.GetMask("Board")))
                {
                    Debug.Log("$$$ released on board");
                    g = GameObject.FindGameObjectWithTag("GameManager");

                    GlobalScript j = g.GetComponent<GlobalScript>();
                    Vector3 puObjectPosition = this.PickedUpObject.transform.position;

                    Debug.Log("$$$ released on board at z-Position: " + puObjectPosition.z);

                    if (puObjectPosition.z > 0)
                    {
                        // ws1
                        ws = j.workstation1;
                        Debug.Log("$$$ Using WS1: " + j.workstation1.name);
                        j.workstation1.GetPhotonView().RPC("AddIngredient", RpcTarget.Others,
                        this.PickedUpObject.GetComponent<PhotonView>().ViewID);
                    }
                    else if (puObjectPosition.z < 0)
                    {
                        ws = j.workstation2;
                        Debug.Log("$$$ Using WS2" + j.workstation2.name);
                        j.workstation2.GetPhotonView().RPC("AddIngredient", RpcTarget.Others,
                        this.PickedUpObject.GetComponent<PhotonView>().ViewID);
                    }

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

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Game")))
            {
                Debug.Log("$$$ raycast update calling");
                GlobalScript j = g.GetComponent<GlobalScript>();

                Debug.Log("$$$ I found: " + hit.transform.gameObject.name + ".");

                Debug.Log("$$$ checking with gameObject for viewID: "
                           + hit.transform.gameObject.GetComponent<PhotonView>().ViewID);

                this.PickUpOrUpdateObject(hit);

                this.lastRay = ray;
            } else
            {

                this.PickUpOrUpdateObject(hit);

                this.lastRay = ray;
            }

            
            //if (this.PickedUpObject != null)
            //{
            //    Debug.Log("$$$ Dragging object");

            //}
            


            //else if (this.m_RaycastManager.Raycast(touchPosition, PickUpIngredient.s_Hits, TrackableType.PlaneWithinPolygon))
            //{
            //    // Raycast hits are sorted by distance, so the first one
            //    // will be the closest hit.
            //    var hitPose = PickUpIngredient.s_Hits[0].pose;
            //    this.CreateOrUpdateObject(hitPose.position, hitPose.rotation);
            //}
        }

        private void PickUpOrUpdateObject(RaycastHit hit)
        {
            if (this.PickedUpObject == null || this.PickedUpObject != hit.transform.gameObject)
            {
                this.PickedUpObject = hit.transform.gameObject;
                Debug.Log("$$$ calling pick up ingredient");
                g.GetPhotonView().RPC("TakeIngredientAway", RpcTarget.Others,
                    this.PickedUpObject.GetComponent<PhotonView>().ViewID);

                this.PickedUpObject.GetComponent<PhotonView>().TransferOwnership(PhotonNetwork.LocalPlayer.ActorNumber);

            }
            this.PickedUpObject.transform.position = hit.point;

        }

        //private void UpdateObject(RaycastHit hit)
        //{
        //}

        protected override void OnPress(Vector3 position)
        {
            this.pressed = true;
            Debug.Log("$$$Pressed");
        }

        protected override void OnPressCancel()
        {
            this.pressed = false;
            Debug.Log("lifted");
            //this.PickedUpObject = null;
        }
    }
}
