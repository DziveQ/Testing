using UnityEngine;
using System.Collections;

public class Atm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform atmViewpoint;
    [SerializeField] private Transform cardViewpoint;
    [SerializeField] private GameObject card;

    [Header("Settings")]
    [SerializeField] private float DistanceToInteract = 3f;
    [Range(1f, 3f)]
    [SerializeField] private float cardSens = 1.5f; // Sensitivity for card movement

    private PlayerController playerController;
    private Camera mainCamera;
    private bool isUsingATM = false;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private bool isHoldingCard = false;
    private bool cardInserted = false;

    void Start() {
        mainCamera = Camera.main;
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerController = player.GetComponent<PlayerController>();
    }

    void Update() {
        float distance = Vector3.Distance(transform.position, playerController.transform.position);

        if (distance < DistanceToInteract) {
            if (!isUsingATM) {
                if (Input.GetKeyDown(KeyCode.E)) { // Start Card Insertion
                    isUsingATM = true;
                    CardInsertion();
                }
            }
            else { // Is interacting with ATM
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)) { // Exit ATM
                    isUsingATM = false;
                    ExitATMView();
                }

                if (isHoldingCard && !cardInserted) { //Card is beeing grabbed and inserted
                    HandleCardMovement();
                }

                if (!cardInserted && card.transform.localPosition.z >= -0.48f) { //Card fully inserted
                    cardInserted = true;
                    EnterATMView(); 
                }
            }
        }
    }

    private void CardInsertion() {
        card.SetActive(true); // Enable the card object
        originalCamPos = mainCamera.transform.position;
        originalCamRot = mainCamera.transform.rotation;
        StartCoroutine(MoveCamera(mainCamera.transform.position, cardViewpoint.position, mainCamera.transform.rotation, cardViewpoint.rotation, 0.4f));

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.enabled = false;

        isHoldingCard = true;
    }

    private void HandleCardMovement() {
        if (Input.GetMouseButton(0)) { // Left mouse button is held down
            float mouseZ = Input.GetAxis("Mouse Y"); // up/down to push/pull
            Vector3 localPos = card.transform.localPosition;

            localPos.z += mouseZ * Time.deltaTime * cardSens; // tweak speed as needed
            localPos.z = Mathf.Clamp(localPos.z, -0.64f, -0.48f); // clamp range

            card.transform.localPosition = localPos;
        }
    }

    private void EnterATMView() {
        StartCoroutine(MoveCamera(cardViewpoint.position, atmViewpoint.position, cardViewpoint.rotation, atmViewpoint.rotation, 0.4f));
    }

    public void ExitATMView() {
        card.SetActive(false); // Disable the card object
        card.transform.localPosition = new Vector3(0.3252f, 0.7195f, -0.64f); // Reset card position
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.enabled = true;

        isHoldingCard = false;
        cardInserted = false;

        StartCoroutine(MoveCamera(mainCamera.transform.position, originalCamPos, mainCamera.transform.rotation, originalCamRot, 0.4f));
    }

    private IEnumerator MoveCamera(Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float duration) {
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            mainCamera.transform.position = Vector3.Lerp(fromPos, toPos, t);
            mainCamera.transform.rotation = Quaternion.Slerp(fromRot, toRot, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.position = toPos;
        mainCamera.transform.rotation = toRot;
    }
}
