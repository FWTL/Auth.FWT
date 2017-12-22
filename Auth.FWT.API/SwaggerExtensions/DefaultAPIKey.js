$(document).ready(function () {
    $('#input_apiKey').val("fa6cb983-f51a-4ca8-a005-9279fc77826e");
    addApiKeyAuthorization();

    function addApiKeyAuthorization() {
        var key = encodeURIComponent($('#input_apiKey')[0].value);
        if (key && key.trim() !== "") {
            var apiKeyAuth = new SwaggerClient.ApiKeyAuthorization("X-ApiKey", key, "header");
            window.swaggerUi.api.clientAuthorizations.add("X-ApiKey", apiKeyAuth);
            log("added key " + key);
        }
    }
});

$("#input_apiKey").on("change keyup paste", function () {
    addApiKeyAuthorization();
})