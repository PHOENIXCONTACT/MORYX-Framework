// Copyright (c) 2020, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

namespace Marvin.Tools.Wcf
{
    /// <summary>
    /// The type of a WCF binding
    /// </summary>
    public enum BindingType
    {
        /// <summary>
        /// BasicHttp is ideal to communcate with a varying, huge number of clients distributed all across the company. 
        /// Since updates and notifications can only be polled it is best suited for clients that do not require low latency and fast response times.
        /// </summary>
        BasicHttp,

        /// <summary>
        /// The proprietary protocol Net.TCP is a fast and high reponsive. 
        /// The full duplex communication structure also provides responsive interaction and event notification. 
        /// It should be used within machines or setups of a few clients with permanent connection which requires a low latenxy and fast response time. 
        /// </summary>
        NetTcp
    }
}
