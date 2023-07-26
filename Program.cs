using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace FoxAristaXVideo {
    public class NIC {
        public string Description { get; set; }
        public string MAC { get; set; }
        public DateTime Timestamp { get; set; }
        public long Rx { get; set; }
        public long Tx { get; set; }
    }

    public class XVideoInterface {
        public string Device { get; set; }
        public string Model { get; set; }
        public NIC NIC { get; set; }
    }

    public class FoxXVideoDriver{
        public string MAC;
        double currentRxBitrate = 0;
        double currentTxBitrate = 0;

        public class BitrateCalculator {
            public void CalculateBitrates(List<XVideoInterface> xVideoDataList) {
                int pollingRate = 2;
                double rxBitrate = 0;
                double txBitrate = 0;
                int count = 0;
                foreach (var xVideoData in xVideoDataList) {
                    double rxBitrate = rxBitrate + (xVideoData.NIC.Rx * 8);
                    double txBitrate = txBitrate + (xVideoData.NIC.Tx * 8);
                    count++;
                }
                rxBitrate = rxBitrate / count * pollingRate;
                txBitrate = txBitrate / count * pollingRate;
                currentRxBitrate = rxBitrate;
                currentTxBitrate = txBitrate;
            }
        }
    }

    class Program {
        static void Main()
        {
            // Read json file with data
            using StreamReader reader = new("data.json");
            var foxJson = reader.ReadToEnd();
            var jarray = JArray.Parse(foxJson);
            List<XVideoInterface> xVideoDataList = new List<XVideoInterface>();
            // Parse json objects as XVideoInterface objects
            foreach(var item in jarray) {
                XVideoInterface xVideoData = item.ToObject<XVideoInterface>();
                xVideoDataList.Add(xVideoData);
            }
            List<string> uniqueMAC = new List<string>();
            List<FoxXVideoDriver> foxDriverList = new List<FoxXVideoDriver>();
            // Create an object for each device with a different MAC address
            foreach(XVideoInterface xVideoData in xVideoDataList) {
                if (uniqueMAC.Count == 0 || !uniqueMAC.Contains(xVideoData.MAC)) {
                    FoxXVideoDriver foxDriver = new FoxXVideoDriver();
                    foxDriver.MAC = xVideoData.MAC;
                    foxDriverList.Add(foxDriver);
                }
            }
            BitrateCalculator bitrateCalculator = new BitrateCalculator();
            bitrateCalculator.CalculateBitrates(xVideoDataList);
        }
    }
}
// steps I would take to improve if given more time:
// One pass through the data in the json file: initialize new devices when a new MAC address is received and update the bitrates stored in each device