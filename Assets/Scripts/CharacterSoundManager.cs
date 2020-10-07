using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace AudioKid { 
    public class CharacterSoundManager : SoundManager
    {
        [Header("Set in Inspector")]
        public PlayerController character;
        
        public Sound jumpSound;
        public Sound chargeSound;
        public Sound[] walkSound;

        private void OnEnable() {
            character.jumpEvent += PlayJumpSound;
            character.chargeStartEvent += PlayChargeSound;
            character.chargeStopEvent += StopChargeSound;
            character.leftFootTouchingGroundEvent += PlayLeftWalkSound;
            character.rightFootTouchingGroundEvent += PlayRightWalkSound;
        }
        private void OnDisable() {
            character.jumpEvent -= PlayJumpSound;
            character.chargeStartEvent -= PlayChargeSound;
            character.leftFootTouchingGroundEvent -= PlayLeftWalkSound;
            character.rightFootTouchingGroundEvent -= PlayRightWalkSound;
        }
        public void PlayJumpSound(float jumpCharge) {
            jumpSound.Play();
        }

        public void PlayChargeSound() {
            chargeSound.Play();
            print("Charging...");
        }
        public void StopChargeSound() {
            chargeSound.Stop();
            print("Stop Charge...");
        }

        private void PlayLeftWalkSound() {
            walkSound[0].Play();
        }
        private void PlayRightWalkSound() {
            walkSound[1].Play();
        }

        protected override void Init() {
            base.Init();
            jumpSound.Init(source);
            chargeSound.Init(source);
            walkSound[0].Init(source);
            walkSound[1].Init(source);
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
