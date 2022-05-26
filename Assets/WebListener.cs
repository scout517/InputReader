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

    private const int PORT = 5005;

    // Start is called before the first frame update
    void Start()
    {
        Thread t = new Thread(() => Listener(GetLocalIP()));
        t.Start();
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
                Debug.Log("Waiting for broadcast");
                Byte[] packet = listener.Receive(ref groupEP);
                string jsonStr = Encoding.UTF8.GetString(packet);
                if (isDictionary(jsonStr))
                {
                    Dictionary<string, object> data = DeserializePacket(jsonStr);
                    IterateDictionary(data);
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

    // Takes the recieved packet and deserializes it into Dictionary object
    private Dictionary<string, object> DeserializePacket(string jsonStr)
    {
        jsonStr = jsonStr.Substring(jsonStr.IndexOf("{"));
        jsonStr = jsonStr.Substring(0, jsonStr.IndexOf("}") + 1);
        return JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonStr);
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

}
