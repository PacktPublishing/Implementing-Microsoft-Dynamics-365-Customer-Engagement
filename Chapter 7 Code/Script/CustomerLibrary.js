var HIMBAP = window.Sdk || {};
(function () {

    this.customertypeOnChange = function (executionContext) {
        //get formContext
        var formContext = executionContext.getFormContext();
        //validate if field is available and it's value is nont null
        if (formContext.getAttribute("customertypecode") != null &&
            formContext.getAttribute("customertypecode").getValue() !== null) {
            //get customer type value
            var customertype = formContext.getAttribute("customertypecode").getValue();
            if (customertype == 3) {
                //show manager field
                formContext.getControl("him_manager").setVisible(true);
            }
            else {
                //hide manager field
                formContext.getControl("him_manager").setVisible(false);
            }


        }
    },
        this.primarContactOnChange = function (executionContext) {
           
            var formContext = executionContext.getFormContext();
            //get entity id
        if (formContext.getAttribute("primarycontactid") != null && formContext.getAttribute("primarycontactid").getValue() != null) {
            var contactid = formContext.getAttribute("primarycontactid").getValue()[0].id.substring(1, 37);

            Xrm.WebApi.online.retrieveRecord("contact", contactid, "?$select=emailaddress1").then(
                function success(result) {
                    var emailaddress1 = result["emailaddress1"];
                    formContext.getAttribute("emailaddress1").setValue(emailaddress1);
                },
                function (error) {
                    Xrm.Utility.alertDialog(error.message);
                }
            );
            }
        }

}).call(HIMBAP);