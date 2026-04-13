$views = Join-Path (Split-Path $PSScriptRoot -Parent) "Views"
Get-ChildItem -Path $views -Recurse -Filter *.cshtml | ForEach-Object {
    $c = Get-Content $_.FullName -Raw -Encoding UTF8
    $replacements = @{
        'MYBUSINESS.Models.Customer' = 'EasyInventory.PgData.Entities.Customer'
        'MYBUSINESS.Models.Employee' = 'EasyInventory.PgData.Entities.Employee'
        'MYBUSINESS.Models.Department' = 'EasyInventory.PgData.Entities.Department'
        'MYBUSINESS.Models.Location' = 'EasyInventory.PgData.Entities.Location'
        'MYBUSINESS.Models.MyBusinessInfo' = 'EasyInventory.PgData.Entities.MyBusinessInfo'
        'MYBUSINESS.Models.Payment' = 'EasyInventory.PgData.Entities.Payment'
        'MYBUSINESS.Models.PO' = 'EasyInventory.PgData.Entities.PO'
        'MYBUSINESS.Models.POD' = 'EasyInventory.PgData.Entities.POD'
        'MYBUSINESS.Models.Product' = 'EasyInventory.PgData.Entities.Product'
        'MYBUSINESS.Models.SO' = 'EasyInventory.PgData.Entities.SO'
        'MYBUSINESS.Models.SOD' = 'EasyInventory.PgData.Entities.SOD'
        'MYBUSINESS.Models.Supplier' = 'EasyInventory.PgData.Entities.Supplier'
        'MYBUSINESS.Models.SaleOrderViewModel' = 'ShopOn.Web.Models.SaleOrderViewModel'
        'MYBUSINESS.Models.PurchaseOrderViewModel' = 'ShopOn.Web.Models.PurchaseOrderViewModel'
        'MYBUSINESS.Models.DashboardViewModel' = 'ShopOn.Web.Models.DashboardViewModel'
        'MYBUSINESS.Models.CustomersWiseSaleViewModel' = 'ShopOn.Web.Models.CustomersWiseSaleViewModel'
        'MYBUSINESS.Models.spSOReport_Result' = 'ShopOn.Web.Models.spSOReport_Result'
        'MYBUSINESS.Models.spPOReport_Result' = 'ShopOn.Web.Models.spPOReport_Result'
        'MYBUSINESS.Models.LoginViewModel' = 'ShopOn.Web.Models.LoginViewModel'
        'MYBUSINESS.Models.RegisterViewModel' = 'ShopOn.Web.Models.RegisterViewModel'
        'MYBUSINESS.Models.VerifyCodeViewModel' = 'ShopOn.Web.Models.VerifyCodeViewModel'
        'MYBUSINESS.Models.SendCodeViewModel' = 'ShopOn.Web.Models.SendCodeViewModel'
        'MYBUSINESS.Models.ResetPasswordViewModel' = 'ShopOn.Web.Models.ResetPasswordViewModel'
        'MYBUSINESS.Models.ForgotPasswordViewModel' = 'ShopOn.Web.Models.ForgotPasswordViewModel'
        'MYBUSINESS.Models.ExternalLoginConfirmationViewModel' = 'ShopOn.Web.Models.ExternalLoginConfirmationViewModel'
    }
    foreach ($k in $replacements.Keys) {
        $c = $c.Replace($k, $replacements[$k])
    }
    [System.IO.File]::WriteAllText($_.FullName, $c)
}
Write-Host "views entity/vm replacements done"
