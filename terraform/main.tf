terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.96.0"
    }
  }
}

provider "azurerm" {
  features {
  }
}

resource "azurerm_resource_group" "rg" {
  name     = "func-demo-rg"
  location = "eastus"
}

resource "azurerm_mssql_server" "db-server" {
  name                         = "func-demo-sql-server"
  resource_group_name          = azurerm_resource_group.rg.name
  location                     = "eastus"
  version                      = "12.0"
  administrator_login          = var.sql-admin-login
  administrator_login_password = var.sql-admin-login-password
}

resource "azurerm_mssql_database" "db" {
  name      = "CustomerOnboarding"
  server_id = azurerm_mssql_server.db-server.id

  depends_on = [
    azurerm_mssql_server.db-server
  ]
}

# Create SQL Server firewall rule for Azure resources' access
resource "azurerm_mssql_firewall_rule" "azureservicefirewall" {
  name             = "allow-azure-service"
  server_id        = azurerm_mssql_server.db-server.id
  start_ip_address = "0.0.0.0"
  end_ip_address   = "0.0.0.0"
}

resource "azurerm_application_insights" "insights" {
  name                = "func-demo-app-insights"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  application_type    = "web"
}

resource "azurerm_service_plan" "fn-plan" {
  name                = "func-demo-app-plan"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "eastus"
  os_type             = "Windows"
  sku_name            = "Y1"
}

resource "azurerm_storage_account" "fn-storage" {
  name                     = "funcdemostorageaccount"
  resource_group_name      = azurerm_resource_group.rg.name
  location                 = "eastus"
  account_tier             = "Standard"
  account_replication_type = "LRS"
}

resource "azurerm_windows_function_app" "app" {
  name                = "func-demo-function-app"
  resource_group_name = azurerm_resource_group.rg.name
  location            = "eastus"

  storage_account_name       = azurerm_storage_account.fn-storage.name
  storage_account_access_key = azurerm_storage_account.fn-storage.primary_access_key
  service_plan_id            = azurerm_service_plan.fn-plan.id

  site_config {
    application_stack {
      dotnet_version              = "v8.0"
      use_dotnet_isolated_runtime = true
    }
  }

  identity {
    type = "SystemAssigned"
  }

  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = azurerm_application_insights.insights.instrumentation_key
    "ConnectionStrings:CustomerOnboardingDbConnection" = "Server=tcp:${azurerm_mssql_server.db-server.name}.database.windows.net,1433;Initial Catalog=${azurerm_mssql_database.db.name};Persist Security Info=False;User ID=${var.sql-admin-login};Password=${var.sql-admin-login-password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
    "SCALE_CONTROLLER_LOGGING_ENABLED" = "AppInsights:Verbose"
  }
}