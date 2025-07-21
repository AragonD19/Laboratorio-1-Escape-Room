using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserSystem : MonoBehaviour
{
    [SerializeField] private Transform laserStart; 
    [SerializeField] private Transform laserEnd;   
    [SerializeField] private LayerMask obstacleMask;

    [SerializeField] private float onTime = 2f;    
    [SerializeField] private float offTime = 2f;
    [SerializeField] private int damage = 100;


    private float timer = 0f;
    private bool isLaserOn = true;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        
        timer += Time.deltaTime;

        if (isLaserOn && timer >= onTime)
        {
            isLaserOn = false;
            timer = 0f;
            lineRenderer.enabled = false;
        }
        else if (!isLaserOn && timer >= offTime)
        {
            isLaserOn = true;
            timer = 0f;
            lineRenderer.enabled = true;
        }

        
        if (isLaserOn && laserStart != null && laserEnd != null)
        {
            Vector3 start = laserStart.position;
            Vector3 end = laserEnd.position;
            Vector3 direction = (end - start).normalized;

            Ray ray = new Ray(start, direction);
            RaycastHit hit;

            Vector3 finalEnd = end;

            if (Physics.Raycast(ray, out hit, Vector3.Distance(start, end), obstacleMask))
            {
                finalEnd = hit.point;

                if (hit.collider.CompareTag("Player"))
                {
                    PlayerHealth health = hit.collider.GetComponent<PlayerHealth>();
                    if (health != null && isLaserOn && health.CanTakeDamage())
                    {
                        health.TakeDamage(damage);
                    }
                }
            }
            else
            {
                Debug.DrawLine(start, end, Color.green);
            }

            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, finalEnd);
        }
    }
}
