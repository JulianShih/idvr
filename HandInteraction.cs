using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Valve.VR;

public class HandInteraction : MonoBehaviour
{
    public enum HandType{
        LeftHand = 0,
        RightHand = 1
    }
    public HandType handType;
    public CharacterInteraction characterInteraction;
    private GameObject touchObject;
    private SteamVR_Input_Sources handSource;
    //YiCi
    [System.NonSerialized]
    public bool touchOthersHand;
    [System.NonSerialized]
    public GameObject avatarOfOtherHand;

    void Start()
    {
        //Assign the hand source (left or right hand)
        handSource = SteamVR_Input_Sources.LeftHand;
        if(this.handType == HandType.RightHand) handSource = SteamVR_Input_Sources.RightHand;
    }

    void Update()
    {
        if(this.touchObject != null) checkGrabObject(this.touchObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(characterInteraction.playerGameObject != null){
            if(characterInteraction.playerGameObject.GetComponent<PhotonView>().IsMine){      
                //Check if the user touch the grabbable object 
                if(other.CompareTag("Grabbable")){
                    this.touchObject = other.gameObject;
                }
                //Check if the user touch any avatar's hand
                if(other.CompareTag("AvatarHand")){
                    //Check whether the hand is the same user's other hand, if it is, then nothing happens
                    if(other.gameObject.GetComponent<HandInteraction>().characterInteraction == this.characterInteraction) return;
                    //If it is other users' hand, then activate the highfive effect
                    Vector3 pos = (this.transform.position + other.gameObject.transform.position) / 2.0f;
                    this.characterInteraction.playerGameObject.GetComponent<PlayerVisualEffect>().activateHighFiveEffect(pos);
                    //YiCi If it is other user's hand
                    touchOthersHand = true;
                    avatarOfOtherHand = other.gameObject;
                }
                //Check if multiple user touch the table
                if (other.CompareTag("Table")) {
                    other.gameObject.GetComponent<TableInteraction>().touched++;
                    if(other.gameObject.GetComponent<TableInteraction>().checkMultiTouched()) {
                        Vector3 pos = new Vector3(other.gameObject.transform.position.x, this.transform.position.y, other.gameObject.transform.position.z);
                        this.characterInteraction.playerGameObject.GetComponent<PlayerVisualEffect>().activateLightEffect(pos);
                    }
                }
            } 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Grabbable")){
            this.touchObject = null;
        }
        if (other.CompareTag("Table")){
            other.gameObject.GetComponent<TableInteraction>().touched--;
        }
            //YiCi
            if (other.CompareTag("AvatarHand"))
        {
            touchOthersHand = false;
        }
    }
    private void checkGrabObject(GameObject touchedObject){
        //Check the grab action
        SteamVR_Action_Boolean grab = SteamVR_Input.GetBooleanAction("default", "GrabPinch");
        
        //If the user activate the grab action, then grab the object
        if(grab.GetStateDown(handSource)){
            pickObject(touchedObject);
        }
        if(grab.GetStateUp(handSource)){
            dropObject(touchedObject);
        }
    }
    private void pickObject(GameObject other){
        //Pick up the object
        other.GetComponent<PhotonView>().RequestOwnership();
        other.transform.position = this.transform.position;
        this.GetComponent<FixedJoint>().connectedBody = other.GetComponent<Rigidbody>();
    }

    private void dropObject(GameObject other){
        this.GetComponent<FixedJoint>().connectedBody = null;
        //Assign the velocity and the angular velocity from the controller (SteamVR_Input_Pose)
        SteamVR_Action_Pose pose = SteamVR_Input.GetPoseAction("default", "Pose");
        other.GetComponent<Rigidbody>().velocity = pose.GetVelocity(handSource);
        other.GetComponent<Rigidbody>().angularVelocity = pose.GetAngularVelocity(handSource);
    }
}
