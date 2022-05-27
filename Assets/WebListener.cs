using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;

public class WebListener : MonoBehaviour
{
    [SerializeField] bool testing = false;
    [SerializeField] string filePath = "";
    [SerializeField] int remainingTests = 0;

    private const int PORT = 5005;
    private RetrievalTest testFile;
    private Queue<Dictionary<string, object>> testQueue;

    // Start is called before the first frame update
    void Start()
    {
        testFile = new RetrievalTest();
        Thread t = new Thread(() => Listener(GetLocalIP()));
        t.Start();
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

    // Obtains the local machine IP address and returns it
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

    public void Listener(IPAddress ipAddress)
    {
        UdpClient listener = new UdpClient(PORT);
        IPEndPoint groupEP = new IPEndPoint(ipAddress, PORT);
        Debug.Log(groupEP.Port);
        try
        {
            while (true)
            {
                Byte[] packet = listener.Receive(ref groupEP);
                string jsonStr = Encoding.UTF8.GetString(packet);
                if (isDictionary(jsonStr))
                {
                    Dictionary<string, object> data = DeserializePacket(jsonStr);
                    if(data != null){
                        IsTesting(data);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        finally
        {
            listener.Close();
        }
    }

    private void IsTesting(Dictionary<string, object> data)
    {
        // Tests recieved dictionary if testing is in progress
        if (remainingTests != 0)
        {
            if (!testFile.TestPacket(data))
            {
                Debug.Log("Test Failed!");
                Application.Quit();
            }
            remainingTests--;
        }
    }

    // Takes the recieved packet and deserializes it into Dictionary object
    private Dictionary<string, object> DeserializePacket(string jsonStr)
    {
        jsonStr = jsonStr.Substring(jsonStr.IndexOf("{"));
        Debug.Log(jsonStr);
        if(jsonStr.LastIndexOf("}") != jsonStr.Length - 1){
            jsonStr = jsonStr.Substring(0, jsonStr.LastIndexOf("}") + 1);
        }
        try{
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
        }
        catch(Exception e)
        {
            Debug.Log(e.ToString());
            Application.Quit();
            return null;
        }
    }

    // Iterates through a dictionary printing all of its keys and values
    private void IterateDictionary(Dictionary<string, object> data)
    {
        foreach (KeyValuePair<string, object> entry in data)
        {
            Debug.Log(entry.Key + ": " + entry.Value);
        }
    }

    // Checks to make sure a string meets basic requirements to be a dictionary before
    // Work in Porgress
    private Boolean isDictionary(string data)
    {
        return data.Contains("{") && data.Contains("}");
    }

    // Tests the data retrieved from the most recent packet
    private bool testDataRetrieval()
    {
        return true;
    }

}
