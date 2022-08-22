using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Explorer : MonoBehaviour
{
    public Material mat;
    public Vector2 pos;
    public float scale=3.25f;
    public float angle;
    public GameObject ControlsPanel;

    public Slider RepeatSlider;
    public TMP_Text RepeatText;


    public Slider RSlider;
    public TMP_Text RText;
    public Slider GSlider;
    public TMP_Text GText;
    public Slider BSlider;
    public TMP_Text BText;

    public Slider ColorSlider;
    public TMP_Text ColorText;

    public Toggle RGBToggle;
    public Toggle IterateOverTimeToggle;

    public TMP_InputField MaxIterationField;
    public TMP_InputField IterationFrequencyField;

    public TMP_InputField XModField;
    public TMP_InputField YModField;

    public bool isIterating = false;
    public bool isBreathing = false;
    bool isClimbing = false;

    decimal iterationFrequency = .1M;

    decimal maxIter = 255;
    public TMP_InputField MinIterationField;
    decimal minIter = 1;
    decimal currentIter = 0;
    float iterationTimer = 0;

    float XMod =1;
    float YMod =1;

    private void UpdateShader()
    {
        float aspect = (float)Screen.width / (float)Screen.height;
        float scaleX = scale;
        float scaleY = scale;
        

        if (aspect > 1f)
        {
            scaleY /= aspect;
        }
        else
        {
            scaleX *= aspect;
        }
        mat.SetVector("_Area", new Vector4(pos.x, pos.y, scaleX, scaleY));
        mat.SetFloat("_Angle", angle);
        mat.SetFloat("_Repeat", RepeatSlider.value*RepeatSlider.value * RepeatSlider.value);
        mat.SetFloat("_RValue", RSlider.value * RSlider.value);
        mat.SetFloat("_GValue", GSlider.value * GSlider.value);
        mat.SetFloat("_BValue", BSlider.value * BSlider.value);
        mat.SetFloat("_Color", ColorSlider.value);
     


        if (isIterating)
        {
            iterationTimer += Time.deltaTime;
            if(iterationTimer > (float)iterationFrequency )
            {
                if (isBreathing)
                {//breathing
                    if (currentIter == maxIter && isClimbing)
                    {
                        isClimbing = false;
                    }
                    if(currentIter == minIter && !isClimbing)
                    {
                        isClimbing = true;
                        
                    }
                    if (isClimbing)
                    {
                        mat.SetFloat("_Iter", (float)currentIter);
                        currentIter += 1;
                        iterationTimer = 0;
                    }
                    else
                    {
                        mat.SetFloat("_Iter", (float)currentIter);
                        currentIter -= 1;
                        iterationTimer = 0;
                    }
                }
                else
                {//not breathing
                    mat.SetFloat("_Iter", (float)currentIter);
                    currentIter += 1;
                    iterationTimer = 0;
                    if (currentIter >= maxIter)
                    {
                        currentIter = minIter;
                    }
                }
            }
            

        }

    }

    private void HandleInputs()
    {
        if (Input.GetKey(KeyCode.E)) //Expand
        {
            angle -= .01f;
        }
        if (Input.GetKey(KeyCode.Q)) //Expand
        {
            angle += .01f;
        }


        if (Input.GetKey(KeyCode.Minus) || Input.GetAxis("Mouse ScrollWheel") < 0) //Expand
        {
            scale *= 1.010f;
        }
        if ((Input.GetKey(KeyCode.Equals) || Input.GetAxis("Mouse ScrollWheel") > 0)&&scale>0) //Expand
        {
            scale *= .990f;
        }
        if (Input.GetKeyDown(KeyCode.F12)){ 

            HideColorControls();
        }

        Vector2 dir = new Vector2(.0025f *scale, 0);
       float s = Mathf.Sin(angle);
        float c = Mathf.Cos(angle);

        dir = new Vector2(dir.x * c - dir.y * s, dir.x * s + dir.y * c);
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) //Expand
        {
            pos -= dir;
        }
        if (Input.GetKey(KeyCode.RightArrow)|| Input.GetKey(KeyCode.D)) //Expand
        {
            pos += dir;
        }

        dir = new Vector2(-dir.y, dir.x);
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) //Expand
        {
            pos += dir;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) //Expand
        {
            pos -= dir;
        }
       
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        HandleInputs();
        UpdateShader();
        UpdateUIStats();
    }

    public void HideControls()
    {

    }

    public void HideColorControls()
    {
        ControlsPanel.SetActive(!ControlsPanel.activeSelf);
    }

    public void UpdateUIStats()
    {
        RepeatText.text = "" + RepeatSlider.value * RepeatSlider.value * RepeatSlider.value;

        RText.text = "" + RSlider.value*RSlider.value;
        GText.text = "" + GSlider.value * GSlider.value;
        BText.text = "" + BSlider.value * BSlider.value;
        ColorText.text = "" + ColorSlider.value;
    }
    public void ToggleColorType()
    {
        if (RGBToggle.isOn)
        {
            mat.SetFloat("_IsGradient", 0);
        }
        else
        {
            mat.SetFloat("_IsGradient", 2);
        }
        
    }

    public void SetMaxIterations()
    {
        
        if (decimal.TryParse(MaxIterationField.text, out maxIter))
        {
            if(maxIter > 2000)
            {
                maxIter = 2000;
            }
            if (maxIter <= minIter && isIterating)
            {
                maxIter = minIter + 1;
            }
            maxIter += (decimal).0001;
            if (!isIterating)
            {
                mat.SetFloat("_Iter", (float)maxIter);
            }
        }
    }

    public void SetMinIterations()
    {
        if (decimal.TryParse(MinIterationField.text, out minIter))
        {
            if (minIter <1)
            {
                minIter = 1;
            }
            if(minIter >= maxIter)
            {
                minIter = maxIter - 1;
            }
            minIter += (decimal).0001;
           
        }
    }
    public void ToggleIterationOverTime()
    {
        if (IterateOverTimeToggle.isOn)
        {
            isIterating = true;
            if (minIter >= maxIter)
            {
                minIter = 1;
                maxIter = 100;
            }
            iterationTimer = 0;
            currentIter = minIter;
            mat.SetFloat("_Iter", (float)currentIter);
           
        }
        else
        {
            isIterating = false;
            iterationTimer = 0;
        }


    }
    public void SetIterationFrequency()
    {
        if (decimal.TryParse(IterationFrequencyField.text, out iterationFrequency))
        {
            if(iterationFrequency < (decimal).000001)
            {
                iterationFrequency = (decimal).000001;
            }
        }
    }

    public void SetModulationFloats()
    {
        float.TryParse(XModField.text, out XMod);
        float.TryParse(YModField.text, out YMod);
        mat.SetFloat("_XMod", XMod);
        mat.SetFloat("_YMod", YMod);
    }
    public void ToggleBreathing()
    {
        isBreathing = !isBreathing;
    }
}
