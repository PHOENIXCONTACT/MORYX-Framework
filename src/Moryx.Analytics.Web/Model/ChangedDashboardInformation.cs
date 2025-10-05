// Copyright (c) 2023, Phoenix Contact GmbH & Co. KG
// Licensed under the Apache License, Version 2.0

using Moryx.Analytics.Server;

namespace Moryx.Analytics.Web.Model
{
  public class ChangedDashboardInformation
  {
    public string OriginalUrl { get; set; }
    public DashboardInformation ChangedDashboard { get; set; }
  }
}

