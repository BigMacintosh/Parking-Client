using System;
using System.Collections.Generic;
using SceneUtilities;
using UnityEngine;
using UI;

namespace Vehicle
{
    public class Vehicle : MonoBehaviour
    {
        public VehicleProperties vehicleProperties;
        private VehicleDriver driver;
        private UIController uicontroller;
        public List<DriveWheel> driveWheels;


        public void SetControllable()
        {
            // Add DriveController to car
            driver  = gameObject.AddComponent<VehicleDriver>();

            driver.accel = 20;
            driver.maxSpeed = 50;
            driver.driveWheels = driveWheels;
            driver.maxSteer = 30;
            driver.driftFactor = 3f;
            driver.setAcceptInput(true);

            // Set camera to follow car
            var camera = FindObjectOfType<CameraFollowController>();
            camera.ObjectToFollow = transform;

            var minimap = FindObjectOfType<MinimapController>();
            minimap.ObjectToFollow = transform;

            //Link with UIController
            uicontroller = FindObjectOfType<UIController>();
            uicontroller.vehicle = this;
        }

        public VehicleDriver getDriver()
        {
            return driver;
        }
    }
    
    // Stores properties for each type of car
    // Allows drive controller to change based on different properties.
    [Serializable]
    public class VehicleProperties
    {
        // Scale from 1 - 10
        [field: SerializeField] public int SpeedRating { get; private set; }
        [field: SerializeField] public int SteeringRating { get; private set; }
        [field: SerializeField] public int WeightRating { get; private set; }
    }
}