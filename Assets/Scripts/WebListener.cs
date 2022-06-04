using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Pool;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;

public class WebListener : MonoBehaviour
{

    /**
        Structure for the recieved packet.
        The boolean used indicates if the packet has been
        used and can be disposed.
    */
    public struct Packet
    {
        public Packet(Dictionary<string, object> data)
        {
            packet = data;
            used = false;
        }
        public Dictionary<string, object> packet;
        public bool used;
        // Add time stamp
    }

    [SerializeField] bool iterateDictionary = false;
    [SerializeField] bool testing = false;
    [SerializeField] string filePath = "";
    [SerializeField] int remainingTests = 0;

    private const int PORT = 5005;

    private RetrievalTest testFile;
    private Queue<Dictionary<string, object>> testQueue;
    private DataRetrieval dataRetrieval;

    void Start()
    {
        testFile = new RetrievalTest();
        dataRetrieval = gameObject.GetComponent<DataRetrieval>();
        StartCoroutine(Listener(GetLocalIP()));
    }

    void Update()
    {
        // Checks to see if the retrievals are being tested
        if (testing)
        {
            remainingTests = testFile.ParseTestFile(filePath);
            testing = false;
        }
    }

    /**
        This method finds the IP address of the local machine.
        This IP address is then used to open a socket for which data can be sent through.
    */
    private IPAddress GetLocalIP()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.MapToIPv4();
            }
        }
        return null;
    }

    /**
        This method creates a socket using the given IPAddress and a port.
        It then waits for a UDP packet to be sent.
    */
    IEnumerator Listener(IPAddress ipAddress)
    {
        UdpClient listener = new UdpClient(PORT);
        IPEndPoint groupEP = new IPEndPoint(GetLocalIP(), PORT);
        Debug.Log(groupEP.Port);
        while (true)
        {
            if (listener.Available > 0)
            {
                try
                {
                    Byte[] packet = listener.Receive(ref groupEP);
                    string jsonStr = Encoding.UTF8.GetString(packet);
                    Dictionary<string, object> data = DeserializePacket(jsonStr);
                    if (data != null)
                    {
                        Debug.Log("Packet Recieved");
                        dataRetrieval.RecievePacket(data);
                        IsTesting(data);
                        IterateDictionary(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
            yield return null;
        }
    }

    /**
        This method checks if the program is currently testing the data being recieved. 
        If a test is in progress, then it sends the data to the RetrievalTest object and 
        tests the data against what it's supposed to be. If the test fails, then a message 
        is sent to the Unity Debug Log stating that a test failed.
    */
    private void IsTesting(Dictionary<string, object> data)
    {
        // Tests recieved Dictionary if testing is in progress
        if (remainingTests != 0)
        {
            if (!testFile.TestPacket(data))
            {
                Debug.Log("Test Failed!");
            }
            remainingTests--;
        }
    }

    /**
        This method takes the recieved UDP packet and turns it into a Dictionary.
        It first finds where the Dictionary starts and ends by looking for the substrings 
        "{\"" and "}". It then uses the Newtonsoft.Json library to convert the string in a 
        Dictionary containing string-object key-value pairs.
    */
    private Dictionary<string, object> DeserializePacket(string jsonStr)
    {
        jsonStr.Replace("\0", string.Empty); // Get rid of all null characters
        jsonStr = jsonStr.Substring(jsonStr.IndexOf("{\"")); // Finds first bracket
        if (jsonStr.LastIndexOf("}") != jsonStr.Length - 1)
        {  // Finds second bracket
            jsonStr = jsonStr.Substring(0, jsonStr.LastIndexOf("}") + 1);
        }
        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr); // Converts Json to Dictionary
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
            return null;
        }
    }

    // Iterates through a dictionary printing all of its keys and values
    private void IterateDictionary(Dictionary<string, object> data)
    {
        if (!iterateDictionary) { return; }
        foreach (KeyValuePair<string, object> entry in data)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

}
