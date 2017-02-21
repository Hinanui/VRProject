﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour {

	public int _score,_bonuscombo,_swipecounter;
	public float _bonustime;
    public GameObject m_pTxtPrefab;
    public float m_fSpawnDistance = 50f;

	private void increaseScore(int val) {
		_score += val * _bonuscombo;
	}

	public void swipeHappen(Vector3 _vPos) {
		_swipecounter++;
		if (_bonustime > 0) 
		{
			_bonustime += 2;
		}

		if (_swipecounter < 5) {
			increaseScore(100);
            SpawnScoreText(100, transform.position + transform.forward * m_fSpawnDistance);
		} 

		else if (_swipecounter < 8) 
		{
			increaseScore(80);
            SpawnScoreText(80, transform.position + transform.forward * m_fSpawnDistance);
        }

		else if (_swipecounter < 11) 
		{
			increaseScore(50);
            SpawnScoreText(50, transform.position + transform.forward * m_fSpawnDistance);
        }

		else if (_swipecounter < 13) 
		{
            increaseScore(20);
            SpawnScoreText(20, transform.position + transform.forward * m_fSpawnDistance);
        }

		else if (_swipecounter > 13) 
		{
			increaseScore(1);
            SpawnScoreText(1, transform.position + transform.forward * m_fSpawnDistance);
        }
	}

	public void throwhappen(Vector3 _vPos)
	{
		
		if (_bonustime == 0) {
			_bonustime += 5;
            SpawnBonusText(5, _vPos);
		}
        else
        {
            _bonuscombo += 2;
            SpawnBonusText(2, _vPos);
        } 
			
	}

	// Use this for initialization
	void Start () {

		_score = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (_bonustime != 0) 
		{
			_bonustime = _bonustime - Time.deltaTime;
		}

		if (_bonustime < 0) 
		{
			_bonustime = 0;
		}
		
	}

    void SpawnScoreText(int _iValue, Vector3 _vPos)
    {
        GameObject pTxt = GameObject.Instantiate(m_pTxtPrefab, _vPos, Quaternion.identity);
        pTxt.GetComponent<InitializeScoreTxt>().SetTxt(_iValue, "+" + _iValue.ToString());
    }

    void SpawnBonusText(int _iValue, Vector3 _vPos)
    {
        GameObject pTxt = GameObject.Instantiate(m_pTxtPrefab, _vPos, Quaternion.identity);
        pTxt.GetComponent<InitializeScoreTxt>().SetTxt(_iValue, "Bonus X" + _iValue.ToString());
    }
}
