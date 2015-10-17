(function() {
    if (typeof (EventSource) !== "undefined") {
        var scripts = document.getElementsByTagName('script');
        var index = scripts.length - 1;
        var myScript = scripts[index];
        var scriptUrl = myScript.src;
        var streamUrl = scriptUrl.substring(0, scriptUrl.length - 13) + "stream"; // 13 being the length of string "BrowserLog.js"
        var source = new EventSource(streamUrl);
        source.onmessage = function(event) {
            console.log(event.data);
        };
        source.addEventListener("DEBUG", function(event) {
            console.debug(event.data);
        });
        source.addEventListener("INFO", function(event) {
            console.info(event.data);
        });
        source.addEventListener("WARN", function(event) {
            console.warn(event.data);
        });
        source.addEventListener("ERROR", function(event) {
            console.error(event.data);
        });
    } else {
        alert("Your browser does not support SSE, hence BrowserLog will not work properly");
    }
})();