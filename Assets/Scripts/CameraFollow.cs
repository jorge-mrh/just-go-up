using UnityEngine;

public class CameraFollow : MonoBehaviour {
    [SerializeField] private Transform target;
    [SerializeField] private GameObject wallLeft;
    [SerializeField] private GameObject wallRight;
  
    private void Start()
    {
        if (wallLeft != null && wallRight != null)
        {
            Camera cam = Camera.main;
            
            // Get screen edges in world space
            Vector3 leftEdge = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
            Vector3 rightEdge = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));
            
            // Position walls at screen edges
            Vector3 wallLeftPos = wallLeft.transform.position;
            wallLeftPos.x = leftEdge.x;
            wallLeftPos.y = transform.position.y; // Follow camera Y
            wallLeft.transform.position = wallLeftPos;
            
            Vector3 wallRightPos = wallRight.transform.position;
            wallRightPos.x = rightEdge.x;
            wallRightPos.y = transform.position.y;
            wallRight.transform.position = wallRightPos;
        }
    }
    
    void LateUpdate() {
            Vector3 wallLeftPos = wallLeft.transform.position;
            wallLeftPos.y = transform.position.y;
            wallLeft.transform.position = wallLeftPos;
            
            Vector3 wallRightPos = wallRight.transform.position;
            wallRightPos.y = transform.position.y;
            wallRight.transform.position = wallRightPos;
    }
}