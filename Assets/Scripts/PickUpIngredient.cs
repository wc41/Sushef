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

        private GameObject g;


        /// <summary>
        /// The object instantiated as a result of a successful raycast intersection with a plane.
        /// </summary>
        public GameObject PickedUpObject { get; private set; }

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
            if (Pointer.current == null || this.pressed == false || !this.CanPlace)
                return;

            var touchPosition = Pointer.current.position.ReadValue();

            // Ensure we are not over any UI element.
            var uiButtons = FindObjectOfType<UIButtons>();
            if (uiButtons != null && (uiButtons.IsPointOverUI(touchPosition)))
                return;

            // Raycast against layer "GroundPlane" using normal Raycasting for our artifical ground plane.
            // For AR Foundation planes (if enabled), we use AR Raycasting.
            var ray = Camera.main.ScreenPointToRay(touchPosition);

            Debug.Log("$$$ ray spawned");
            g = GameObject.FindGameObjectWithTag("GameManager");

            g.GetPhotonView().RPC("Raycast", RpcTarget.Others, ray);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000, LayerMask.GetMask("Game")))
            {
                Debug.Log("$$$ raycast update calling");
                // GlobalScript j = g.GetComponent<GlobalScript>();

                Debug.Log("$$$ I found: " + hit.transform.gameObject.name + ".");

                /*for (int i = 0; i < j.ingredients.Length; i++)
                {
                    if (j.ingredients[i].GetComponent<PhotonView>().ViewID == 
                        hit.transform.gameObject.GetComponent<PhotonView>().ViewID)
                    {
                        this.UpdateOrPickUpObject(hit);
                        break;
                    }
                }*/
            }
            //else if (this.m_RaycastManager.Raycast(touchPosition, PickUpIngredient.s_Hits, TrackableType.PlaneWithinPolygon))
            //{
            //    // Raycast hits are sorted by distance, so the first one
            //    // will be the closest hit.
            //    var hitPose = PickUpIngredient.s_Hits[0].pose;
            //    this.CreateOrUpdateObject(hitPose.position, hitPose.rotation);
            //}
        }

        private void UpdateOrPickUpObject(RaycastHit hit)
        {
            if (this.PickedUpObject == null || this.PickedUpObject != hit.transform.gameObject)
            {
                this.PickedUpObject = hit.transform.gameObject;
                Debug.Log("$$$ calling pick up ingredient");
                g.GetPhotonView().RPC("TakeIngredientAway", RpcTarget.Others, this.PickedUpObject.GetComponent<PhotonView>().ViewID);

            }

            this.PickedUpObject.transform.position = hit.point;
        }

        protected override void OnPress(Vector3 position)
        {
            this.pressed = true;
            Debug.Log("$$$Pressed");
        }

        protected override void OnPressCancel()
        {
            this.pressed = false;
            Debug.Log("lifted");
        }
    }
}
