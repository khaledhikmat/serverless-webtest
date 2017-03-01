# Login to Azure first
Login-AzureRmAccount

# Select the subscription
Get-AzureRmSubscription | select SubscriptionName
$subscr = "your-own"
Select-AzureRmSubscription -SubscriptionName $subscr

# 1. create a new resource group in west US
New-AzureRmResourceGroup -Name WebTest4WestUS -Location "West US"

# 1.5. deploy the template to the west us resource group
New-AzureRmResourceGroupDeployment -Name WebTest4WestUSDeployment -ResourceGroupName WebTest4WestUS `
  -TemplateFile azuredeploy.json  

# 2. create a new resource group in west europe
New-AzureRmResourceGroup -Name WebTest4WestEurope -Location "West Europe"

# 2.5. deploy the template to the west europe resource group
New-AzureRmResourceGroupDeployment -Name WebTest4WestEuropeDeployment -ResourceGroupName WebTest4WestEurope `
  -TemplateFile azuredeploy.json

# 3. create a new resource group in West Japan
New-AzureRmResourceGroup -Name WebTest4WestJapan -Location "Japan West"

# 3.5. deploy the template to the west japan resource group
New-AzureRmResourceGroupDeployment -Name WebTest4WestJapanDeployment -ResourceGroupName WebTest4WestJapan `
  -TemplateFile azuredeploy.json    

# 4. create a new resource group in South Brazil
New-AzureRmResourceGroup -Name WebTest4SouthBrazil -Location "Brazil South"

# 4.5. deploy the template to the south brazil resource group
New-AzureRmResourceGroupDeployment -Name WebTest4SouthBrazilDeployment -ResourceGroupName WebTest4SouthBrazil `
  -TemplateFile azuredeploy.json  
  
######

# Delete the resource groups
Remove-AzureRmResourceGroup -Name WebTest4WestUS -Force
Remove-AzureRmResourceGroup -Name WebTest4WestEurope -Force
Remove-AzureRmResourceGroup -Name WebTest4WestJapan -Force
Remove-AzureRmResourceGroup -Name WebTest4SouthBrazil -Force
   