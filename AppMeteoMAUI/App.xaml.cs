﻿using AppMeteoMAUI.Service;
namespace AppMeteoMAUI;

public partial class App : Application
{
    public static PreferitiRepository PreferitiRepo { get; set; }
    public App(PreferitiRepository repo)
	{
		InitializeComponent();
		MainPage = new AppShell();
        PreferitiRepo = repo;
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt+QHFqVkNrWU5BaV1CX2BZfVl1QWlcfU4QCV5EYF5SRHJfR1xmSnpWdUdiXXs=;Mgo+DSMBPh8sVXJ1S0d+X1RPc0BHQmFJfFBmRmlae1R1dkUmHVdTRHRcQlljTH9WdUFmWn1cd3A=;ORg4AjUWIQA/Gnt2VFhhQlJBfVpdWHxLflF1VWBTfFp6cVdWACFaRnZdQV1nSXtSc0ZnXHxcdHRR;MTYzMjE0OEAzMjMxMmUzMTJlMzMzNUpFekJCcUdCUnlKdnk5YkRlbHl5RC8rS3ZEWDRFak42dmZoc1BndVVYeE09;MTYzMjE0OUAzMjMxMmUzMTJlMzMzNU9HemFYZ283d0hLSGZoMUVDcGJ6bjl4dk5aN0MxYy9FeUhHbHZXZVVvVGs9;NRAiBiAaIQQuGjN/V0d+XU9Hc1RHQmZWfFN0RnNadV10flBEcDwsT3RfQF5jTX5Wd0BgXXpdcn1cQg==;MTYzMjE1MUAzMjMxMmUzMTJlMzMzNWJMUlJTZS9wNWloeWhZajBZSzMzZ3lJaUkyN1ZnZVB5R0d0ZXkzT05UcUk9;MTYzMjE1MkAzMjMxMmUzMTJlMzMzNWk2Mm1HYTVGTjJiTUZId1haTzUrMWZKTkRqZkxPNmprMEluL09LYndnUHM9;Mgo+DSMBMAY9C3t2VFhhQlJBfVpdWHxLflF1VWBTfFp6cVdWACFaRnZdQV1nSXtSc0ZnXHxddXBR;MTYzMjE1NEAzMjMxMmUzMTJlMzMzNWpjYzVnYVc2cWZDdER4eVJNcFhrL0ZtdGs0L0RYNitqWnVqMVhvSXpDR0k9;MTYzMjE1NUAzMjMxMmUzMTJlMzMzNWpEaHhwbnJHWVVIV1JaV2xNWi9mRHFsR2hrTUg5bTZrNlJlL1ZLcVFIWkk9;MTYzMjE1NkAzMjMxMmUzMTJlMzMzNWJMUlJTZS9wNWloeWhZajBZSzMzZ3lJaUkyN1ZnZVB5R0d0ZXkzT05UcUk9");
    }
}
