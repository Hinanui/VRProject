﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Manipulation : MonoBehaviour {

    public GameObject m_pObject;
    bool m_bHasObject;

	const int nbRegisteredLastPosition = 2;
    
    [SerializeField]
    float m_fVacuumSpeed = 1f;
    [SerializeField]
    float m_fVacuumStopDistance = 5f;

    LineRenderer m_pLineRenderer;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
	
    private SteamVR_TrackedObject trackedObject;
    private SteamVR_Controller.Device device;

	private List<Vector3> lastPositions = new List<Vector3>();

	RaycastHit hit = new RaycastHit();
	Vector3 raycastHitPos;

	

	void Start () 
	{
        m_pLineRenderer = GetComponent<LineRenderer>();
	}
	
	void Update () 
	{
		#region Recherche du steam VR
		if (trackedObject == null) 
		{
			trackedObject = GetComponent<SteamVR_TrackedObject> ();
		}

        if(trackedObject)
	        device = SteamVR_Controller.Input((int)trackedObject.index);
		#endregion
       
		
		LaserPointer();
        GrabObject();

		updateLastPosition(lastPositions, this.gameObject);

		#region Definir la position finale du laser
		if (m_bHasObject) 
		{
			
			DrawLineRenderer (m_pObject.transform.position);
		} 
		else 
		{
			
			DrawLineRenderer (transform.position + transform.forward * 50);
		}
		#endregion
	}

    void DrawLineRenderer(Vector3 lastPos)
    {
        m_pLineRenderer.SetPosition(0, transform.position);
		m_pLineRenderer.SetPosition(1, lastPos);
    }

	void updateLastPosition(List<Vector3> vecList, GameObject obj)
	{
		if (vecList.Count >= nbRegisteredLastPosition) {
			vecList.RemoveAt(nbRegisteredLastPosition - 1);
		}

		vecList.Insert(0, obj.transform.position);
	}

	Vector3 velocityFromLastPositions() 
	{
		return lastPositions[0] - lastPositions[nbRegisteredLastPosition - 1];
	}

    void LaserPointer()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        if (Physics.Raycast(transform.position, fwd, out hit, 100))
        {
            if (hit.collider.gameObject.tag == "Piece" && m_pObject == null)
            {
                m_pObject = hit.collider.gameObject;
                if(m_pObject.GetComponent<shaderGlow>() != null)              
                    m_pObject.GetComponent<shaderGlow>().lightOn();
                
            }

            if (m_pObject != null && hit.collider.gameObject != m_pObject && !m_bHasObject)
            {
                if (m_pObject.GetComponent<shaderGlow>() != null)
                    m_pObject.GetComponent<shaderGlow>().lightOff();
                m_pObject = null;
            }
        }
        else if (m_pObject != null && !m_bHasObject)
        {
            if (m_pObject.GetComponent<shaderGlow>() != null)
                m_pObject.GetComponent<shaderGlow>().lightOff();
            m_pObject = null;
        }
    }

    void GrabObject()
    {
        MouseInput();
        VRControllerInput();
        Shoot();
        
        if (m_pObject != null && m_bHasObject)
        {
            if (Vector3.Distance(transform.position, m_pObject.transform.position) > m_fVacuumStopDistance)
            {
				m_pObject.transform.position = Vector3.Lerp(m_pObject.transform.position, transform.position + transform.forward*2f, Time.deltaTime * m_fVacuumSpeed);
				m_pObject.transform.rotation = Quaternion.Lerp(m_pObject.transform.rotation, transform.rotation, Time.deltaTime * 2f);

                if (device != null)
                    device.TriggerHapticPulse(700);
            }
            else if (Vector3.Distance(transform.position, m_pObject.transform.position) <= m_fVacuumStopDistance)
            {
				m_pObject.transform.position = transform.position + transform.forward*2f;
				m_pObject.transform.rotation = transform.rotation;

                if (device != null)
                    device.TriggerHapticPulse(300);

            }
		}
        
    }

    void Shoot()
    {
        if (Input.GetMouseButtonDown(1) && m_pObject != null && m_bHasObject)
        {
            m_bHasObject = false;
            m_pObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_pObject.GetComponent<Rigidbody>().AddForce(Camera.main.transform.forward * 200f, ForceMode.Impulse);
            m_pObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void MouseInput()
    {
        if (Input.GetMouseButtonDown(0) && m_pObject != null)
        {
            m_bHasObject = true;
            m_pObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_pObject.GetComponent<Rigidbody>().useGravity = false;
        }
        if (Input.GetMouseButtonUp(0) && m_pObject != null)
        {
            m_bHasObject = false;
            m_pObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            m_pObject.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    void VRControllerInput()
    {
        //Has a VR controller detected
        if (device != null)
        {
			if (device.GetPressDown (triggerButton) && m_pObject != null) {
				m_bHasObject = true;
				m_pObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				m_pObject.GetComponent<Rigidbody> ().useGravity = false;
			} 
            if (device.GetPressUp(triggerButton) && m_pObject != null)
            {
				Debug.Log ("Pressed trigger up");
                m_bHasObject = false;
                m_pObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                m_pObject.GetComponent<Rigidbody>().useGravity = true;

                if (!m_bHasObject)
                {
                    m_pObject.GetComponent<Rigidbody>().AddForce(velocityFromLastPositions() * 500, ForceMode.Impulse);
                }
            }

			
        }
    }
}