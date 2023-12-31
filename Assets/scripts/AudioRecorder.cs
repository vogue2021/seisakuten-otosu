﻿using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using static System.BitConverter;
using UnityEngine.UI;
using System.Collections.Generic;

namespace akr.Unity.Audio
{

    public class AudioRecorder : MonoBehaviour
    {
        private const int WaveHeaderSize = 44;
        private const int MicrophoneLengthSec = 10;

        // [SerializeField]
        // private KeyCode recordStartKey = KeyCode.R;
        // [SerializeField]
        // private KeyCode recordStopKey = KeyCode.X;
        [SerializeField]
        private int samplingFrequency = 48000;
        [SerializeField]
        private string microphoneDevice = null;
        [SerializeField]
        private string[] microphoneDevices;

        [SerializeField]
        private bool isRecording;

        private PostFile postfilescript;

        public List<string> filenames;


        private void Awake()
        {
            microphoneDevices = Microphone.devices;
            postfilescript = GetComponent<PostFile>();
        }

        // private void Update()
        // {
        //     if (Input.GetKeyDown(recordStartKey) && isRecording == false)
        //     {
        //         Debug.Log("Start Audio Record");
        //         //var debug_path = $"Assets/Resources/Audio_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.wav";
        //         var path = Application.persistentDataPath + "/Audio_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.wav";
        //         StartCoroutine(WavRecording(null, path));
        //     }
        //     else if (Input.GetKeyDown(recordStopKey) && isRecording == true)
        //     {
        //         isRecording = false;
        //         Debug.Log("Stop Audio Record");
        //     }
        // }

        public void StartRecording()
        {
            if (!isRecording)
            {
                Debug.Log("Start Audio Record");
                string currentTime = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
                filenames.Add(currentTime);

                // var path = Path.Combine(currentTime + ".wav");
                var path = Path.Combine(Application.persistentDataPath, currentTime + ".wav");
                // var path = Application.persistentDataPath + "/Audio_{DateTime.Now:yyyy_MM_dd_HH_mm_ss}.wav";
                StartCoroutine(WavRecording(null, path));
            }
        }

        public void StopRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                Debug.Log("Stop Audio Record");
            }
        }



        public IEnumerator WavRecording(string micDeviceName, string filePath)
        {
            isRecording = true;

            Debug.Log("Audio Record to " + filePath);

            var targetDirectory = Path.GetDirectoryName(filePath);

            if (Directory.Exists(targetDirectory) == false)
            {
                Directory.CreateDirectory(targetDirectory);
            }

            var currentFileStream = new FileStream(filePath, FileMode.Create);

            //header area
            currentFileStream.WriteBytes(new byte[WaveHeaderSize]);

            var microphoneBuffer = new float[MicrophoneLengthSec * samplingFrequency];
            var microphoneAudioClip = Microphone.Start(micDeviceName, true, MicrophoneLengthSec, samplingFrequency);
            int writeHead = 0;
            int writePosition;

            while (isRecording)
            {
                writePosition = Microphone.GetPosition(micDeviceName);

                if (writePosition >= 0 && writeHead != writePosition)
                {
                    WriteWaveBuffer(currentFileStream, writeHead, writePosition, microphoneBuffer, microphoneAudioClip);
                    writeHead = writePosition;
                }
                yield return null;
            }

            writePosition = Microphone.GetPosition(micDeviceName);
            Microphone.End(micDeviceName);

            //write last data
            WriteWaveBuffer(currentFileStream, writeHead, writePosition, microphoneBuffer, microphoneAudioClip);
            //finalize
            WriteWaveHeader(currentFileStream, microphoneAudioClip.channels, samplingFrequency);
            Debug.Log("Audio Record End " + filePath);

            //PostFileの呼び出し
            postfilescript.UploadWavFile(filePath);
        }

        private void WriteWaveBuffer(FileStream fileStream, int head, int position, float[] buffer, AudioClip audioClip)
        {
            audioClip.GetData(buffer, 0);

            for (int i = head; i < position || (head > position && i < buffer.Length); i++)
            {
                fileStream.WriteBytes((short)(buffer[i] * short.MaxValue));
            }
            if (head > position)
            {
                for (int i = 0; i < position; i++)
                {
                    fileStream.WriteBytes((short)(buffer[i] * short.MaxValue));
                }
            }

        }

        private void WriteWaveHeader(FileStream fileStream, int channels, int samplingFrequency)
        {
            var samples = ((int)fileStream.Length - WaveHeaderSize) / 2;

            fileStream.Flush();
            fileStream.Seek(0, SeekOrigin.Begin);

            fileStream.WriteBytes("RIFF");

            //chunk size
            fileStream.WriteBytes((int)fileStream.Length - 8);

            fileStream.WriteBytes("WAVE");

            fileStream.WriteBytes("fmt ");

            // sub chunk
            fileStream.WriteBytes(16);

            //audio format
            fileStream.WriteBytes((ushort)1);

            //channel count
            fileStream.WriteBytes((ushort)channels);

            //sample rate
            fileStream.WriteBytes(samplingFrequency);

            //byte rate
            fileStream.WriteBytes(samplingFrequency * channels * 2);

            //block align
            fileStream.WriteBytes((ushort)(channels * 2));

            //bps
            fileStream.WriteBytes((ushort)16);

            fileStream.WriteBytes("data");

            //sub chunk2
            fileStream.WriteBytes(samples * channels * 2);

            fileStream.Flush();
            fileStream.Close();
        }
    }

    public static class StreamExtensions
    {
        public static void WriteBytes(this Stream stream, byte[] bytes) => stream.Write(bytes, 0, bytes.Length);

        public static void WriteBytes(this Stream stream, int data) => stream.WriteBytes(GetBytes(data));

        public static void WriteBytes(this Stream stream, short data) => stream.WriteBytes(GetBytes(data));

        public static void WriteBytes(this Stream stream, ushort data) => stream.WriteBytes(GetBytes(data));

        public static void WriteBytes(this Stream stream, string data) => stream.WriteBytes(Encoding.UTF8.GetBytes(data));

    }
}