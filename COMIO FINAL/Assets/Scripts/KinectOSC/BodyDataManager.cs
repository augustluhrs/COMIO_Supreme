using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
/*
* takes all incoming OSC messages from TD,
* sorts by address, assigns position and rotation to manager gameobjects in hierarchy
* optional detection features with trigger/range/threshold settings
*
*
* using OSC Jack -- https://github.com/keijiro/OscJack
*/

public class BodyDataManager : MonoBehaviour
{
    // y of the avatar manager should be set by height calibration TODO
    // public float floorScale = 7f; //however we want to scale the incoming position data
    // public float heightScale = 1f;
    
    [Header("DEBUG/TESTING")]
    //test variables
    public bool showDemoCubes = true;
    public GameObject demoCube;
    public float cubeScale = 0.5f;
    public bool isUsingFullSkeleton = false;

    private float fixedUpdateCounter = 0f;
    private const float interval = 1f;

    [Header("JOINT REFERENCES FOR AVATAR")]
    // public GameObject[] jointTrackingSlots; //to assign gameobjects whose transforms we want to match these joints
    public GameObject headTracker;
    public GameObject handLeftTracker;
    public GameObject handRightTracker;
    public GameObject pelvisTracker;
    public GameObject footLeftTracker;
    public GameObject footRightTracker;


    //need to store a reference to all joint children of our avatar
    //can be assigned to meshes or avatar joints (todo, check anim stuff)
    //always using local Position because assumes nested hierarchy, both for joints and avatar to avatarManager
    // **ACTUALLY JK, Kinect sends "world position" relative to sensor so still local position relative to manager, but no longer nested
    GameObject[] joints;
    string[] jointNames; //have to do this because can't check gameObject.name in the message threads
    Vector3[] jointPositions; //same as above, localposition vectors
    Vector3[] mappedJointPositions; //after calibration, testing diff between jointPositions

    // need rotations
    //Quaternion[] jointRotations;
    int jointIndex = 0; //dumb but idk, for now

    [Header("FEATURES")]
    public bool isRenderFaderOn = true;
    public GameObject[] renderFaderPoints;
    public GameObject renderFaderPoint1; //update with direction?
    public GameObject renderFaderPoint2;
    public GameObject renderFaderPoint3;
    public GameObject renderFaderPoint4;
    public GameObject renderFaderPoint5;
    public GameObject renderFaderPoint6;
    public GameObject renderFaderPoint7;
    public GameObject renderFaderPoint8;

    public float[] renderFaderValues;
    // public float renderFaderValue1 = 0.5f;
    // public float renderFaderValue2 = 0.5f;
    // public float renderFaderValue3 = 0.5f;
    // public float renderFaderValue4 = 0.5f;
    // public float renderFaderValue5 = 0.5f;
    // public float renderFaderValue6 = 0.5f;
    // public float renderFaderValue7 = 0.5f;
    // public float renderFaderValue8 = 0.5f;


    [Header("OSC DATA AND CALIBRATION")]
    OscServer _server;
    public Vector3 incomingPelvisPos = new Vector3(0, 0, 0); //hmm
    public Vector3 incomingRightHandPos = new Vector3(0, 0, 0); //hmm
    public Vector3 incomingLeftHandPos = new Vector3(0, 0, 0); //hmm
    public bool isCalibrating = false; //false for testing, using captured calibration

    CalibrationProfileManager calib;

    void Awake(){
        // calibrationProfileManager = GetComponent<CalibrationProfileManager>();
        calib = GetComponent<CalibrationProfileManager>(); //shortening just for map function legibility

        //need to set up the joints before the server tries to access them
        //checks for joint label in the children of the avatar manager
        joints = new GameObject[32];
        jointNames = new string[32];
        jointPositions = new Vector3[32];
        mappedJointPositions = new Vector3[32];
        IterateTransformHierarchy(transform);

        renderFaderPoints = new GameObject[8];
        renderFaderPoints[0] = renderFaderPoint1;
        renderFaderPoints[1] = renderFaderPoint2;
        renderFaderPoints[2] = renderFaderPoint3;
        renderFaderPoints[3] = renderFaderPoint4;
        renderFaderPoints[4] = renderFaderPoint5;
        renderFaderPoints[5] = renderFaderPoint6;
        renderFaderPoints[6] = renderFaderPoint7;
        renderFaderPoints[7] = renderFaderPoint8;

        renderFaderValues = new float[8];
        for (int i = 0; i < 8; i++)
        {
            renderFaderValues[i] = 0.5f;
        }

        // jointTrackingSlots = new GameObject[32];
        // renderFaderPoints = new GameObject[8];

        //update the editor fields
        // for (int i = 0; i < 32; i ++) {
        //     jointTrackingSlots[i] = new GameObject();
        // }

        // for (int i = 0; i < 8; i ++) {
        //     renderFaderPoints[i] = new GameObject();
        // }

        //uncheck in inspector if don't want cubes to show
        if (showDemoCubes){
            if (isUsingFullSkeleton)
            {
                AddDemoCubes(true); 
            }
            else 
            {
                AddDemoCubes(false);
            }
            
        }
    }

    void IterateTransformHierarchy(Transform parentTransform){
        //thanks chat-GPT
        foreach (Transform childTransform in parentTransform)
        {
            // add the game object to the joints array so that we can update position (or create mesh component)
            // Debug.Log("Child name: " + childTransform.name);

            //gotta be a better way, guess I should just use a list, but w/e
            if (childTransform.gameObject.tag == "joint"){
                joints[jointIndex] = childTransform.gameObject;
                jointNames[jointIndex] = childTransform.gameObject.name;
                jointPositions[jointIndex] = new Vector3(0, 0, 0); //cant remember the .zero way
                jointIndex++;
                
                // If the child has more children, recursively iterate through them
                if (childTransform.childCount > 0)
                {
                    IterateTransformHierarchy(childTransform);
                }
            }
        }
    }

    //not great, but placeholder for now. prob won't work if bools change during play
    void AddDemoCubes(bool isFullSkel){
        if (isFullSkel)
        {
            foreach (GameObject joint in joints){
                //add a demo cube for visualizing the joints before we connect these to an avatar
                //now using prefab on the Avatar Layer
                GameObject cubeObject = Instantiate(demoCube, joint.transform.position, joint.transform.rotation);
                cubeObject.transform.parent = joint.transform;
                cubeObject.transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
                Destroy(cubeObject.GetComponent<BoxCollider>());
            }
        } else 
        {
            for (int i = 0; i < 32; i++)
            {
                if (i == 0 || i == 8 || i == 15 || i == 21 || i == 25 || i == 26) 
                {
                    GameObject cubeObject = Instantiate(demoCube, joints[i].transform.position, joints[i].transform.rotation);
                    cubeObject.transform.parent = joints[i].transform;
                    cubeObject.transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
                    Destroy(cubeObject.GetComponent<BoxCollider>());
                }
            }
        }
        
    }

    void Start()
    {
        _server = new OscServer(9000); // Port number

        _server.MessageDispatcher.AddCallback(
            "", // OSC address --empty is all incoming messages
            (string address, OscDataHandle data) => {
                //TODO should see if there's a more optimized way of doing this that decreases latency
                
                //see incoming data and and addresses
                // Debug.Log(string.Format("({0}, {1})",
                //     address,
                //     data.GetElementAsFloat(0)));

                //parse the OSC message by body joint and parameter 
                //ex. if msg is "/p1/pelvis:tx:0.12", joint label = pelvis, param = tx, val = 0.12
                string[] splitAddy = address.Split('/');
                string label = "";
                string param = "";
                float val = 0f;
                bool isBody = false;
               
                if (splitAddy[1] == "p1"){ //make sure its a body tracking message
                    if (splitAddy[2] != "id") { //don't need for now
                        isBody = true; //just don't want to have the rest of this stuff nested in here
                        string[] parts = splitAddy[2].Split(':');
                        
                        label = parts[0];
                        param = parts[1];
                        val = data.GetElementAsFloat(0);
                        // foreach (string part in parts){
                        //     Debug.Log(string.Format("({0}, {1})",
                        //     "part",
                        //     part));
                        // } 
                    }
                }



                if (!isBody) {return;}
                //if past this point, we have a joint label, param, and val

                //have to store all these as separate references because can't check gameObjects in message threads
                //update the position of the gameObjects accordingly
                if (isUsingFullSkeleton)
                {
                    for (int i = 0; i < 32; i++){
                        if (jointNames[i] == label){
                            Vector3 jPos = jointPositions[i];
                            if (param == "tx"){
                                jPos = new Vector3 (val, jPos.y, jPos.z);
                            } else if (param == "ty") {
                                jPos = new Vector3 (jPos.x, val, jPos.z);
                            } else if (param == "tz") {
                                jPos = new Vector3 (jPos.x, jPos.y, val);
                            }
                            jointPositions[i] = jPos;
                        }
                    }
                } else 
                { //for smaller joint range (just pelvis, hands, feet, head)
                    for (int i = 0; i < 32; i++)
                    {
                        if (jointNames[i] == label && (i == 0 || i == 8 || i == 15 || i == 21 || i == 25 || i == 26)) 
                        {
                           Vector3 jPos = jointPositions[i];
                            if (param == "tx"){
                                jPos = new Vector3 (val, jPos.y, jPos.z);
                            } else if (param == "ty") {
                                jPos = new Vector3 (jPos.x, val, jPos.z);
                            } else if (param == "tz") {
                                jPos = new Vector3 (jPos.x, jPos.y, val);
                            }
                            jointPositions[i] = jPos; 
                        }
                    }
                }
                

                //update pre-mapping for calibration
                if (isCalibrating)
                {
                    incomingPelvisPos = jointPositions[0];
                    incomingRightHandPos = jointPositions[15];
                    incomingLeftHandPos = jointPositions[8]; //TODO double check
                } else
                {
                    // //map the positions to the calibrated scale
                    // for (int i = 0; i < 32; i++)
                    // {
                    //     Vector3 jointPos = jointPositions[i];
                    //     Debug.Log(jointPos);
                    //     float mapped_x, mapped_y, mapped_z;
                    //     mapped_x = Map(jointPos.x, calib.kinect_x_min, calib.kinect_x_max, calib.stage_x_min, calib.stage_x_max);
                    //     mapped_y = Map(jointPos.y, calib.kinect_y_min, calib.kinect_y_max, calib.stage_y_min, calib.stage_y_max);
                    //     mapped_z = Map(jointPos.z, calib.kinect_z_min, calib.kinect_z_max, calib.stage_z_min, calib.stage_z_max);
                    //     mappedJointPositions[i] = new Vector3(mapped_x, mapped_y, mapped_z);
                    //     Debug.Log(mappedJointPositions[i]);
                    // }
                }    
            }
        );

        //delete
        Debug.Log(Vector3.Distance(renderFaderPoint1.transform.position, renderFaderPoint3.transform.position));
    }

     // Update is called once per frame
    void Update()
    {
    
        // this is so the render fader value only gets sent processed once per second
        fixedUpdateCounter += Time.fixedDeltaTime;


        //update gameObject transforms -- TODO check for optimization
        if (isCalibrating)
        {
            //display incoming kinect positions without scaling
            for(int i = 0; i < 32; i++){
                joints[i].transform.localPosition = jointPositions[i];
            }
        } else 
        {
            if (isUsingFullSkeleton)
            {
                //map the positions to the calibrated scale
                for (int i = 0; i < 32; i++)
                {
                    // Debug.Log(i);
                    Vector3 jointPos = jointPositions[i];
                    // Debug.Log(jointPos);
                    float mapped_x, mapped_y, mapped_z;
                    mapped_x = Map(jointPos.x, calib.kinect_x_min, calib.kinect_x_max, calib.stage_x_min, calib.stage_x_max);
                    mapped_y = Map(jointPos.y, calib.kinect_y_min, calib.kinect_y_max, calib.stage_y_min, calib.stage_y_max);
                    mapped_z = Map(jointPos.z, calib.kinect_z_min, calib.kinect_z_max, calib.stage_z_min, calib.stage_z_max);
                    mappedJointPositions[i] = new Vector3(mapped_x, mapped_y, mapped_z);
                    // Debug.Log(mappedJointPositions[i]);
                }
                for(int i = 0; i < 32; i++){
                    joints[i].transform.localPosition = mappedJointPositions[i];
                }
            } else 
            {
                //map the positions to the calibrated scale
                for (int i = 0; i < 32; i++)
                {
                    if (i == 0 || i == 8 || i == 15 || i == 21 || i == 25 || i == 26) 
                    {
                        Vector3 jointPos = jointPositions[i];
                        float mapped_x, mapped_y, mapped_z;
                        mapped_x = Map(jointPos.x, calib.kinect_x_min, calib.kinect_x_max, calib.stage_x_min, calib.stage_x_max);
                        mapped_y = Map(jointPos.y, calib.kinect_y_min, calib.kinect_y_max, calib.stage_y_min, calib.stage_y_max);
                        mapped_z = Map(jointPos.z, calib.kinect_z_min, calib.kinect_z_max, calib.stage_z_min, calib.stage_z_max);
                        mappedJointPositions[i] = new Vector3(mapped_x, mapped_y, mapped_z);
                    }
                    
                }
                for(int i = 0; i < 32; i++){
                    if (i == 0 || i == 8 || i == 15 || i == 21 || i == 25 || i == 26) 
                    {
                        joints[i].transform.localPosition = mappedJointPositions[i];
                    }
                }
            }


/// WHERE WE NEED TO ASSIGN THE LOCATION OF THE TRACKERS

            // headTracker.transform.localPosition =   new Vector3(mappedJointPositions[26].x * magnitudeVar, mappedJointPositions[26].y * magnitudeVar, mappedJointPositions[26].z * magnitudeVar);
            // pelvisTracker.transform.localPosition = new Vector3(mappedJointPositions[0].x * magnitudeVar, mappedJointPositions[0].y * magnitudeVar, mappedJointPositions[0].z * magnitudeVar);
            // handLeftTracker.transform.localPosition =  new Vector3(mappedJointPositions[15].x * magnitudeVar, mappedJointPositions[15].y * magnitudeVar, mappedJointPositions[15].z * magnitudeVar);
            // handRightTracker.transform.localPosition =  new Vector3(mappedJointPositions[8].x * magnitudeVar, mappedJointPositions[8].y * magnitudeVar, mappedJointPositions[8].z * magnitudeVar);
            // footLeftTracker.transform.localPosition =  new Vector3(mappedJointPositions[21].x * magnitudeVar, mappedJointPositions[21].y * magnitudeVar, mappedJointPositions[21].z * magnitudeVar);
            // footRightTracker.transform.localPosition = new Vector3(mappedJointPositions[25].x * magnitudeVar, mappedJointPositions[25].y * magnitudeVar, mappedJointPositions[25].z * magnitudeVar);
            // Debug.Log(mappedJointPositions[26]);

            // headTracker.transform.localPosition = mappedJointPositions[26];
            // // pelvisTracker.transform.localPosition = new Vector3(0, 5, 0);
            // pelvisTracker.transform.localPosition = mappedJointPositions[0];
            // handLeftTracker.transform.localPosition =  mappedJointPositions[15];
            // handRightTracker.transform.localPosition = mappedJointPositions[8];
            // footLeftTracker.transform.localPosition = mappedJointPositions[21];
            // footRightTracker.transform.localPosition = mappedJointPositions[25];


            headTracker.transform.localPosition = mappedJointPositions[26];
            pelvisTracker.transform.localPosition =  mappedJointPositions[0];
            handLeftTracker.transform.localPosition =  mappedJointPositions[15];
            handRightTracker.transform.localPosition = mappedJointPositions[8];
            footLeftTracker.transform.localPosition = mappedJointPositions[21];
            footRightTracker.transform.localPosition = mappedJointPositions[25];
            
        }
        

        //feature tests:

        //render fader distance checks
        //hardcoding distance range based on assumption that the floor grid is 9x9
        //default distance is 18 for 1:3 and 8:7 (width/height) or 25.5 for 2:4 and 5:6 (diagonals)
        //using sq magnitude instead of distance for performance, so 18 = 324, 25.5 = 650
        //wait nvm, just make it a circle where the diameter is 18 to simplify. TODO check with dunk
        //check max closeness of both hands
        //output normalized value where 0 is both hands are on opposite side
        //1 is either hand is on top of the land point
        if (isRenderFaderOn && fixedUpdateCounter > interval)
        {
            RenderFade();
            fixedUpdateCounter = 0;
        }
        
    }

    // Map function to remap a value from one range to another
    float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Ensure the value is within the source range
        // value = Mathf.Clamp(value, fromMin, fromMax); //need this?

        // Calculate the mapped value
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    void OnDestroy()
    {
        _server?.Dispose();
        _server = null;
    }

     void RenderFade()
 {
            Vector3 leftHandPos = mappedJointPositions[8];
            Vector3 rightHandPos = mappedJointPositions[15];
            for (int i = 0; i < 8; i++)
            {
                float faderValue = 0.1f;
                //get whichever hand is closer, then map that hand's offset vector's magnitude
                Vector3 leftOffset = renderFaderPoints[i].transform.localPosition - leftHandPos;
                Vector3 rightOffset = renderFaderPoints[i].transform.localPosition - rightHandPos;
                float leftSqr = leftOffset.sqrMagnitude;
                float rightSqr = rightOffset.sqrMagnitude;
                if (leftSqr < rightSqr)
                {
                    float distMag = 324f - leftSqr;
                    faderValue = Map(distMag, 200f, 0f, 0f, 1f);
                } else 
                {
                    float distMag = 324f - rightSqr;
                    faderValue = Map(distMag, 200f, 0f, 0f, 1f);
                }
                renderFaderValues[i] = faderValue;
            }


}

    }



