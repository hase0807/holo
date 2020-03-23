using UnityEngine;
using Leap.Unity;

namespace Es.InkPainter.Sample
{
    public class MousePainter : MonoBehaviour
    {
        /// <summary>
        /// Types of methods used to paint.
        /// </summary>
        [System.Serializable]
        private enum UseMethodType
        {
            RaycastHitInfo,
            WorldPoint,
            NearestSurfacePoint,
            DirectUV,
        }

        [SerializeField]
        private Brush brush;

        [SerializeField]
        private UseMethodType useMethodType = UseMethodType.RaycastHitInfo;

        [SerializeField]
        bool erase = false;

        public GameObject handMod;
        public GameObject Sphere;
        //public GameObject LeapRig;

        private void Update()
        {
            //if (Input.GetMouseButton(0))
            //{
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Vector3 hitosashi = handMod.GetComponent<CapsuleHand>().Hitosashi;
                Sphere.transform.position = hitosashi;
                Vector3 hitoDir = hitosashi - Camera.main.transform.position;
                var ray = new Ray(Camera.main.transform.position, hitoDir);
                Debug.DrawRay(ray.origin, ray.direction, Color.red, 3.0f);
                //Debug.DrawRay(Camera.main.transform.position, hitoDir, Color.red, 2.0f);
                bool success = true;
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo))
                {
                    var paintObject = hitInfo.transform.GetComponent<InkCanvas>();
                    if (paintObject != null)
                        switch (useMethodType)
                        {
                            case UseMethodType.RaycastHitInfo:
                                success = erase ? paintObject.Erase(brush, hitInfo) : paintObject.Paint(brush, hitInfo);
                                break;

                            case UseMethodType.WorldPoint:
                                success = erase ? paintObject.Erase(brush, hitInfo.point) : paintObject.Paint(brush, hitInfo.point);
                                break;

                            case UseMethodType.NearestSurfacePoint:
                                success = erase ? paintObject.EraseNearestTriangleSurface(brush, hitInfo.point) : paintObject.PaintNearestTriangleSurface(brush, hitInfo.point);
                                break;

                            case UseMethodType.DirectUV:
                                if (!(hitInfo.collider is MeshCollider))
                                    Debug.LogWarning("Raycast may be unexpected if you do not use MeshCollider.");
                                success = erase ? paintObject.EraseUVDirect(brush, hitInfo.textureCoord) : paintObject.PaintUVDirect(brush, hitInfo.textureCoord);
                                break;
                        }
                    if (!success)
                        Debug.LogError("Failed to paint.");
                }
            //}
        }

        public void OnGUI()
        {
            if (GUILayout.Button("Reset"))
            {
                foreach (var canvas in FindObjectsOfType<InkCanvas>())
                    canvas.ResetPaint();
            }
        }
    }
}