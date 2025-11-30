using UnityEngine;

namespace System
{
    public class Respawn : MonoBehaviour {
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, 2f);
        }
    }
}