using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TNVirtualKeyboard : MonoBehaviour
{
	
	public static TNVirtualKeyboard instance;
	
	public string words = "";
	
	public GameObject vkCanvas;
	
	public TMP_InputField targetText;

	private bool _hasSetTargetText;

	private int _lastFrame;
    // Start is called before the first frame update
    void Start()
    {
	    _hasSetTargetText = targetText != null;
        instance = this;
		HideVirtualKeyboard();
    }

    public void BindTargetText(TMP_InputField inputField)
    {
	    targetText = inputField;
	    _hasSetTargetText = true;
    }

    public void UnBindTargetText()
    {
	    targetText = null;
	    _hasSetTargetText = false;
	    words = "";
    }
    
	public void KeyPress(string k){
		if (!_hasSetTargetText || HasInputFiredTwice())
		{
			return;
		}
		words += k;
		targetText.text = words;	
	}
	
	public void Del(){
		if (!_hasSetTargetText || HasInputFiredTwice())
		{
			return;
		}
		words = words.Remove(words.Length - 1, 1);
		targetText.text = words;	
	}
	
	public void ShowVirtualKeyboard(){
		vkCanvas.SetActive(true);
	}
	
	public void HideVirtualKeyboard(){
		vkCanvas.SetActive(false);
	}

	private bool HasInputFiredTwice()
	{
		//Hacky solution because why do poke events fire twice?!
		if (_lastFrame == Time.frameCount)
		{
			return true;
		}
		_lastFrame = Time.frameCount;
		return false;
	}
}
