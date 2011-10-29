
window.onload = function () {
    var tick = 0;

    function checkChange(callback) {
        var xmlhttp;
        if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
            xmlhttp = new XMLHttpRequest();
        }
        else {// code for IE6, IE5
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
        xmlhttp.onreadystatechange = function () {
            if (xmlhttp.readyState == 4 && xmlhttp.status == 200) {
                callback(xmlhttp.responseText);
            }
        }

        xmlhttp.open(
            "GET",
            "_autof5_check?tick="
                + tick
                + "&t="
                + new Date().getTime(),
            true);
        xmlhttp.send();
    }

    function receiveChange(serverTick) {
        if (!tick)
            tick = serverTick;
        else if (serverTick && tick < serverTick) {
            window.location.reload();
            tick = serverTick;
        }

        checkChange(receiveChange);
    }

    checkChange(receiveChange);
};