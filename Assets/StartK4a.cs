using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;

using System;
using System.Diagnostics;
using System.Threading.Tasks;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

public class StartK4a : MonoBehaviour
{
    void Start() { azureKinect(); }

    public static void azureKinect()
    {
        _print(true, "start main");
        using (Device device = Device.Open())
        {
            _print(true, "opened device");
            device.StartCameras(new DeviceConfiguration()
            {
                CameraFPS = FPS.FPS30,
                ColorResolution = ColorResolution.Off,
                DepthMode = DepthMode.NFOV_Unbinned,
                WiredSyncMode = WiredSyncMode.Standalone,
            });

            _print(true, "started camera");
            var deviceCalibration = device.GetCalibration();

            //small difference with PointCloud enabled
            //pos: head -0.2916188 -178.0469 853.1077
            //pos: head -5.753897 -183.444 856.1947
            // PointCloud.ComputePointCloudCache(deviceCalibration);

            TrackerConfiguration trackerConfiguration = new TrackerConfiguration() {
                ProcessingMode = TrackerProcessingMode.Gpu,
                SensorOrientation = SensorOrientation.Default
            };
            using (Tracker tracker = Tracker.Create(deviceCalibration, trackerConfiguration))
            {
                _print(true, "tracker created");
                while (true)
                {
                    //_print(true"test0");
                    using (Capture sensorCapture = device.GetCapture())
                    {
                        // Queue latest frame from the sensor. thros System.FieldAccessException
                        tracker.EnqueueCapture(sensorCapture);
                    }
                    _print(true, "init'd Capture sensorCapture");

                    using (Frame frame = tracker.PopResult())
                    {
                        if (frame == null)
                        {
                            _print(true, "frame was null");
                            return;
                        }
                        
                        if (frame.NumberOfBodies < 1)
                        {
                            _print(true, "no bodies");
                            return;
                        }

                        _print(true, "{0} bodies found" + frame.NumberOfBodies);
                        _print(true, "body id: " + frame.GetBodyId(0));                        
                    }
                }
            }
        }
    }

    public static void _print(bool shouldPrint, string msg, bool isWarning=false)
    {
        if (!shouldPrint) return;
        if (isWarning)
        {
            UnityEngine.Debug.LogWarning(msg);
            return;
        }
        UnityEngine.Debug.Log(msg);

    }
}
