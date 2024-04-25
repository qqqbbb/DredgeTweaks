using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tweaks
{
    public class HarvestPOIdisabler : MonoBehaviour
    {
        public int checkDay = -1;
        public bool enabled_ = true;

        public void OnEnable()
        {
            if (checkDay < GameManager.Instance.Time.Day)
            {
                checkDay = GameManager.Instance.Time.Day;
                enabled_ = Config.fishingSpotDisableChance.Value <= UnityEngine.Random.value;
                //Util.Log(this.name + " HarvestPOIdisabler OnEnable " + Util.GetGameTime() + " " + enabled_);
            }
            gameObject.SetActive(enabled_); 

        }
        public void OnDisable()
        {
            //Util.Message(this.name + " HarvestPOIdisabler OnDisable " + this.isActiveAndEnabled);
        }

        //public void OnDestroy()
        //{
        //    Util.Log(this.name + " HarvestPOIdisabler OnDestroy " + Util.GetGameTime());
        //}

        public void OnTriggerEnter(Collider other)
        {
            //if (!Input.GetKey(KeyCode.LeftShift))
                return;
                
            SimpleBuoyantObject sbo = GetComponent<SimpleBuoyantObject>();
            if (sbo != null)
            {
                sbo.enabled = false;
            }
            Cullable cullable = GetComponent<Cullable>();
            if (cullable != null)
            {
                GameManager.Instance.CullingBrain.RemoveCullable(cullable);
                Util.Message(name + " RemoveCullable ");
            }
            //this.transform.position = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            transform.SetParent(null);
            transform.localPosition = Vector3.zero;
            //transform.position = Vector3.zero;
            float x = GameManager.Instance.Player.transform.position.x;
            float y = GameManager.Instance.Player.transform.position.y;
            float z = GameManager.Instance.Player.transform.position.z;
            transform.position = new Vector3(x, y, z);
            Util.Log(this.name + " new pos " + transform.position);
            //transform.SetParent(GameManager.Instance.Player.transform, true);
        }

        public void OnTriggerStay(Collider other)
        {
            //Util.Message(this.name + " HarvestPOIdisabler OnTriggerStay " + other.gameObject.name);
        }
    }
}
