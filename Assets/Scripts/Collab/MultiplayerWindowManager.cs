using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Collab
{
    public class MultiplayerWindowManager : MonoBehaviour
    {
        [SerializeField] private GameObject labelInput;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button createButton;

        private void Start()
        {
            joinButton.onClick.AddListener(JoinRoom);
            createButton.onClick.AddListener(CreateRoom);
        }

        private void JoinRoom()
        {
            var roomId = labelInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_InputField>().text;
            Debug.Log(roomId);

            // TODO
        }

        private void CreateRoom()
        {
            var roomId = labelInput.transform.Find("Text Area/Placeholder").GetComponent<TMP_InputField>().text;
            Debug.Log(roomId);

            // TODO
        }
    }
}