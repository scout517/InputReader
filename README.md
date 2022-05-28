# InputReader
Containes test scripts for sending Dictionary data structues from Python to C#
 
The data is sent through sockets and can be sent a fast as 60 Hertz. The accuracy of the sent data was tested by randomaly generating 100,000 Dictionaries and sending them to C#. This test was conducted 5/27/22. The test resulted in all Dictionaries being sent correctly and not data was lost.

WebListener.cs
The Web Listener is responsible for recieving the data sent. The Web Listener has been tested to recieve UDP packages at a rate of 60 hertz. The test was conducted by sending 100,000 randomly generated Dictionaries and testing if they are equal to what they are supposed to be. 

void Start() -> This method is called before the first frame in a Unity Application. This method first initalizes a RetrievalTest object to be used if a test is conducted. It also creates a Thread object that is linked to the Listener() method and starts the thread. This thread is created to allow Unity to continue working while the WebListener is waiting for a UDP packet to arrive.

void Update() -> This method is called at every frame in a Unity Application. This method is contantly checking if the user indicated they wanto to perform a test. If a test is initiated, then the RetrievalTest object parses a given test file that includes the upcoming data that it will test against.

IPAddress GetLocalIP() -> This method finds the IP address of the local machine. This IP address is then used to open a socket for which data can be sent through.

void Listener(IPAddress) -> This method creates a socket using the given IPAddress and a port. It then waits for a UDP packet to be sent.

void IsTesting() -> This method checks if the program is currently testing the data being recieved. If a test is in progress, then it sends the data to the RetrievalTest object and tests the data against what it's supposed to be. If the test fails, then a message is sent to the Unity Debug Log stating that a test failed.

Dictionary<string, object> DeserializePacket(string) -> This method takes the recieved UDP packet and turns it into a Dictionary. It first finds where the Dictionary starts and ends by looking for the substrings "{\"" and "}". It then uses the Newtonsoft.Json library to convert the string in a Dictionary containing string-object key-value pairs.

void IterateDictionary(Dictionary<string, object> data) -> Iterates through the dictionary printing all of its key-value pairs.

bool isDictionary(string) -> Tests to see if a string can be converted into a Dictionary. Work in progress method.


