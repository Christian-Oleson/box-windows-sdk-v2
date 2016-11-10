﻿using Box.V2.Models.Request;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Box.V2.Models
{
    public class BoxTaskAssignmentRequest 
    {
        /// <summary>
        /// The task this assignment is for.
        /// </summary>
        [JsonProperty(PropertyName = "task")]
        public BoxTaskRequest Task { get; set; }

        /// <summary>
        /// The user this assignment is for. .
        /// </summary>
        [JsonProperty(PropertyName = "assign_to")]
        public BoxAssignmentRequest AssignTo { get; set; }
    }
}
