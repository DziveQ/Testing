using UnityEngine;
using System.Collections;

public class Atm : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform atmViewpoint;
    [SerializeField] private Transform cardViewpoint;

    [Header("Settings")]
    [SerializeField] private float DistanceToInteract = 3f;

    private PlayerController playerController;
    private Camera mainCamera;
    private bool isUsingATM = false;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    void Start() {
        mainCamera = Camera.main;
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        playerController = player.GetComponent<PlayerController>();
    }

    void Update() {
        float distance = Vector3.Distance(transform.position, playerController.transform.position);

        if (distance < DistanceToInteract) {
            if (!isUsingATM) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    CardInsertion();
                    //EnterATMView();
                }
            }
            else {
                if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape)){
                    ExitATMView();
                }
            }
        }
    }

    private void CardInsertion() {
        // Move the camera to the card insertion point
        StartCoroutine(MoveCamera(mainCamera.transform.position, cardViewpoint.position, mainCamera.transform.rotation, cardViewpoint.rotation, 0.4f));
        Debug.Log("Card inserted into ATM.");
        
        // Wait for a moment before entering ATM view
        Invoke("EnterATMView", 1f);
    }

    private void EnterATMView() {
        isUsingATM = true;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerController.enabled = false;

        originalCamPos = mainCamera.transform.position;
        originalCamRot = mainCamera.transform.rotation;

        StartCoroutine(MoveCamera(originalCamPos, atmViewpoint.position, originalCamRot, atmViewpoint.rotation, 0.4f));

        Debug.Log("Entered ATM view.");
    }

    public void ExitATMView() {
        isUsingATM = false;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        playerController.enabled = true;

        StartCoroutine(MoveCamera(mainCamera.transform.position, originalCamPos, mainCamera.transform.rotation, originalCamRot, 0.4f));

        Debug.Log("Exited ATM.");
    }

    private IEnumerator MoveCamera(Vector3 fromPos, Vector3 toPos, Quaternion fromRot, Quaternion toRot, float duration) {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
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
// dd