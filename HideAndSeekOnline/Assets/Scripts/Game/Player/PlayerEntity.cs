using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Project.Game.Player
{
    public class PlayerEntity : NetworkBehaviour
    {
        [SerializeField] private GameObject PlayerInterface;
        [SerializeField] private TextMeshProUGUI nameplate;
        
        [SerializeField] private Behaviour[] componentsToDisable;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                nameplate.text = value;
            }
        }
        private string _name;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            
            if (IsOwner) PlayerInterface.SetActive(true);

            if (!IsLocalPlayer)
            {
                foreach (var component in componentsToDisable)
                {
                    component.enabled = false;
                }
            }
        }
    }
}
