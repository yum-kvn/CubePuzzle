﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

public class Settings : SingletonMonoBehaviour<Settings>
{
// Parameter
	private ControlType controllerType;
	private bool isSoundOn;
	private bool isPadLeft;
	public Vector3 tiltBasePosition;
// NGUI
	public GameObject baseGo;
	public UILabel contollerLabel;
	public UIImageButton soundButton;
	public GameObject padOptionGo;
	public GameObject padHGo;
	public GameObject padVGo;
	public UILabel padButtonLabel;
	public GameObject tiltOptionGo;
	public UISprite arrowSprite;
// db
	private const string KEY_CONTROLLER = "controller_type";
	private const string KEY_SOUND = "sound_on_off";
	private const string KEY_PAD_DIRECTION = "pad_direction";
	private const string KEY_TILT_POS = "tile_position";

	protected override void Awake()
	{
		base.Awake();
		baseGo.gameObject.SetActive(false);

		if (PlayerPrefs.HasKey(KEY_CONTROLLER)) {
			controllerType = (ControlType)PlayerPrefs.GetInt(KEY_CONTROLLER);
		} else {
			controllerType = ControlType.Pad;
		}
		UpdateControllerLabel();

		if (PlayerPrefs.HasKey(KEY_SOUND)) {
			isSoundOn = PlayerPrefs.GetBool(KEY_SOUND);
		} else {
			isSoundOn = true;
		}
		UpdateSoundIcon();

		if (PlayerPrefs.HasKey(KEY_PAD_DIRECTION)) {
			isPadLeft = PlayerPrefs.GetBool(KEY_PAD_DIRECTION);
		} else {
			isPadLeft = true;
		}
		UpdatePad();

		if (PlayerPrefs.HasKey(KEY_TILT_POS)) {
			List<float> _data = PlayerPrefs.GetArray<float>(KEY_TILT_POS);
			tiltBasePosition = new Vector3(_data[0], _data[1], _data[2]);
		} else {
			ResetClick();
		}
	}

	public void Show()
	{
		baseGo.gameObject.SetActive(true);
		gameObject.SetActive(true);

		Top.instance.mainGo.SetActive(false);
	}

	public void Hide()
	{
		PlayerPrefs.Flush();
		gameObject.SetActive(false);
		baseGo.gameObject.SetActive(false);

		Top.instance.mainGo.SetActive(true);
	}

	void FixedUpdate()
	{
		if (isTilt) {
			UpdateTestArrow();
		}
	}

//----------------
// NGUI
//----------------
	void CloseClick()
	{
		Hide();
	}

	void ChangeControllerClick()
	{
		int _typeInt = (int)controllerType;
		_typeInt = (_typeInt+1)%3;
		controllerType = (ControlType)_typeInt;
		PlayerPrefs.SetInt(KEY_CONTROLLER, _typeInt);

		UpdateControllerLabel();
	}

	private void UpdateControllerLabel()
	{
		string _text = "";
		switch (controllerType) {
			case ControlType.Pad:
				_text = "Control Pad";
				padOptionGo.SetActive(true);
				tiltOptionGo.SetActive(false);
			break;
			case ControlType.Swipe:
				_text = "Swipe";
				padOptionGo.SetActive(false);
				tiltOptionGo.SetActive(false);
			break;
			case ControlType.Tilt:
				_text = "Tilt";
				padOptionGo.SetActive(false);
				tiltOptionGo.SetActive(true);
			break;
		}
		contollerLabel.text = _text;
	}

	void SoundClick()
	{
		isSoundOn = !isSoundOn;
		PlayerPrefs.SetBool(KEY_SOUND, isSoundOn);
		UpdateSoundIcon();
	}

private const string SOUND_ON_SPRITE = "btn_sound_on";
private const string SOUND_OFF_SPRITE = "btn_sound_off";
	private void UpdateSoundIcon()
	{
		string _spriteName = isSoundOn ? SOUND_ON_SPRITE : SOUND_OFF_SPRITE;
		soundButton.normalSprite = _spriteName;
		soundButton.hoverSprite = _spriteName;
		soundButton.pressedSprite = _spriteName;
		soundButton.disabledSprite = _spriteName;
	}

	void ChangeControllerSideClick()
	{
		isPadLeft = !isPadLeft;
		PlayerPrefs.SetBool(KEY_PAD_DIRECTION, isPadLeft);
		UpdatePad();
	}

	private void UpdatePad()
	{
		Vector3 _posH = padHGo.transform.localPosition;
		Vector3 _posV = padVGo.transform.localPosition;
		if (isPadLeft) {
			_posH.x = -1 * Mathf.Abs(_posH.x);
			_posV.x = Mathf.Abs(_posV.x);
		} else {
			_posH.x = Mathf.Abs(_posH.x);
			_posV.x = -1 * Mathf.Abs(_posV.x);
		}
		padHGo.transform.localPosition = _posH;
		padVGo.transform.localPosition = _posV;

		_posH = Cube.instance.padHGo.transform.localPosition;
		_posV = Cube.instance.padVGo.transform.localPosition;
		if (isPadLeft) {
			_posH.x = -1 * Mathf.Abs(_posH.x);
			_posV.x = Mathf.Abs(_posV.x);
		} else {
			_posH.x = Mathf.Abs(_posH.x);
			_posV.x = -1 * Mathf.Abs(_posV.x);
		}
		Cube.instance.padHGo.transform.localPosition = _posH;
		Cube.instance.padVGo.transform.localPosition = _posV;

		if (isPadLeft) {
			padButtonLabel.text = "L";
		} else {
			padButtonLabel.text = "R";
		}
	}

	void ResetClick()
	{
		tiltBasePosition = new Vector3(Input.acceleration.x, Input.acceleration.y, 0f);
		PlayerPrefs.SetArray<float>(KEY_TILT_POS, new List<float>(){tiltBasePosition.x,tiltBasePosition.y,tiltBasePosition.z});
	}

private string UP_ARROW = "arrow_up";
private string DOWN_ARROW = "arrow_down";
private string LEFT_ARROW = "arrow_left";
private string RIGHT_ARROW = "arrow_right";
	private void UpdateTestArrow()
	{
		Vector3 dir = Vector3.zero;
		dir.x = Input.acceleration.x;
		dir.y = Input.acceleration.y;
		dir = dir - tiltBasePosition;
		float _x = dir.x;
		float _y = dir.y;

		if (Mathf.Abs(_x) > Mathf.Abs(_y)) {
			if (_x > 0.1) {
				arrowSprite.enabled = true;
				arrowSprite.spriteName = RIGHT_ARROW;
			} else if (_x < -0.1) {
				arrowSprite.enabled = true;
				arrowSprite.spriteName = LEFT_ARROW;
			} else {
				arrowSprite.enabled = false;
			}
		} else {
			if (_y > 0.1) {
				arrowSprite.enabled = true;
				arrowSprite.spriteName = UP_ARROW;
			} else if (_y < -0.1) {
				arrowSprite.enabled = true;
				arrowSprite.spriteName = DOWN_ARROW;
			} else {
				arrowSprite.enabled = false;
			}
		}
	}

//----------------
// enum
//----------------
	public enum ControlType {
		Pad = 0,
		Swipe = 1,
		Tilt = 2,
	}
	public bool isPad {get {return controllerType == ControlType.Pad;}}
	public bool isSwipe {get {return controllerType == ControlType.Swipe;}}
	public bool isTilt {get {return controllerType == ControlType.Tilt;}}
}
