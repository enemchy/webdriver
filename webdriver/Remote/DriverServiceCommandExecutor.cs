﻿// <copyright file="DriverServiceCommandExecutor.cs" company="WebDriver Committers">
// Licensed to the Software Freedom Conservancy (SFC) under one
// or more contributor license agreements. See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership. The SFC licenses this file
// to you under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OpenQA.Selenium.Remote
{
    /// <summary>
    /// Provides a mechanism to execute commands on the browser
    /// </summary>
    internal class DriverServiceCommandExecutor : ICommandExecutor
    {
        private DriverService service;
        private HttpCommandExecutor internalExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout)
            : this(driverService, commandTimeout, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DriverServiceCommandExecutor"/> class.
        /// </summary>
        /// <param name="driverService">The <see cref="DriverService"/> that drives the browser.</param>
        /// <param name="commandTimeout">The maximum amount of time to wait for each command.</param>
        /// <param name="enableKeepAlive"><see langword="true"/> if the KeepAlive header should be sent
        /// with HTTP requests; otherwise, <see langword="false"/>.</param>
        public DriverServiceCommandExecutor(DriverService driverService, TimeSpan commandTimeout, bool enableKeepAlive)
        {
            this.service = driverService;
            this.internalExecutor = new HttpCommandExecutor(driverService.ServiceUrl, commandTimeout, enableKeepAlive);
        }

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="commandToExecute">The command you wish to execute</param>
        /// <returns>A response from the browser</returns>
        public Response Execute(Command commandToExecute)
        {
            Response toReturn = null;
            if (commandToExecute.Name == DriverCommand.NewSession)
            {
                this.service.Start();
            }

            // Use a try-catch block to catch exceptions for the Quit
            // command, so that we can get the finally block.
            try
            {
                toReturn = this.internalExecutor.Execute(commandToExecute);
            }
            finally
            {
                if (commandToExecute.Name == DriverCommand.Quit)
                {
                    this.service.Dispose();
                }
            }

            return toReturn;
        }
    }
}
