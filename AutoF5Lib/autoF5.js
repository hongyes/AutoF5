
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

    function receiveChange(res) {

        if (res) {
            var resObj = eval('(' + res + ')');
            var serverTick = resObj.serverTick;
            var type = resObj.type;
            if (!tick)
                tick = serverTick;
            else if (serverTick && tick < serverTick) {
                if (type == 0)
                    window.location.reload();
                else if (type == 1)
                    updateStylesheets();
                
                tick = serverTick;
            }
        }

        checkChange(receiveChange);
    }

    function updateStylesheets() {
        var i, a, s;
        a = document.getElementsByTagName('link');
        for (i = 0; i < a.length; i++) {
            s = a[i];
            if (s.rel.toLowerCase().indexOf('stylesheet') >= 0 && s.href) {
                var h = s.href.replace(/(&|\?)forceReload=\d*/g, '');
                s.href = h + (h.indexOf('?') >= 0 ? '&' : '?') + 'forceReload=' + (new Date().valueOf());
            }
        }
    }


    checkChange(receiveChange);
};