var PresenceControlbrowseris = new PresenceControlBrowseris();
var PresenceControlObject = null;
var bPresenceControlInited = false;

var PresenceControlURIDictionaryObj = null;
var PresenceControlStatesDictionaryObj = null;

var PresenceControlOrigScrollFunc = null;
var bPresenceControlInScrollFunc = false;


function EnsurePresenceControl() {
    if (!bPresenceControlInited) {
        if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {
            try {
                PresenceControlObject = new ActiveXObject("Name.NameCtrl.1");
            }
            catch (e) {
                alert(e.message);
            };



        }
        bPresenceControlInited = true;
        if (PresenceControlObject) {
            PresenceControlObject.OnStatusChange = PresenceControlOnStatusChange;
        }
    }
    return PresenceControlObject;
}

function PresenceControlOnStatusChange(name, state, id) {
    if (PresenceControlStatesDictionaryObj) {
        var img = PresenceControlGetStatusImage(state);
        if (PresenceControlStatesDictionaryObj[id] != state) {
            PresenceControlUpdateImage(id, img);
            PresenceControlStatesDictionaryObj[id] = state;
        }
    }
}

function PresenceControlShowOOUIMouse() {
    PresenceControlShowOOUI(0);
}

function PresenceControlShowOOUIFocus() {
    PresenceControlShowOOUI(1);
}

function PresenceControlShowOOUI(inputType) {
    if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {
        var obj = window.event.srcElement;
        var objSpan = obj;
        var objOOUI = obj;
        var oouiX = 0,
            oouiY = 0;
        if (EnsurePresenceControl() && PresenceControlURIDictionaryObj) {
            var objRet = PresenceControlGetOOUILocation(obj, false);

            objSpan = objRet.objSpan;
            objOOUI = objRet.objOOUI;
            oouiX = objRet.oouiX;
            oouiY = objRet.oouiY;
            var name = PresenceControlURIDictionaryObj[objOOUI.id];
            if (objSpan) objSpan.onkeydown = PresenceControlHandleAccelerator;
            PresenceControlObject.ShowOOUI(name, inputType, oouiX, oouiY);
        }
    }
}


function PresenceControlHideOOUI() {
    PresenceControlObject.HideOOUI();
}

function PresenceControlGetStatusImage(state) {
    var img = "presence_16-unknown.png";
    switch (state) {
        case 0:
            img = "presence_16-online.png";
            break;
        case 1:
            img = "presence_16-off.png";
            break;
        case 3:
        case 7:
            img = "presence_16-busy.png";
            break;
        case 2:
        case 4:
        case 5:
        case 6:
            img = "presence_16-away.png";
            break;
        case 9:
            img = "presence_16-dnd.png";
            break;
        case 16:
            img = "presence_16-idle-online.png";
            break;
    }
    return img;
}

function PresenceControlUpdateImage(id, img) {
    var obj = document.images(id);
    if (obj) {
        var oldImg = obj.src;
        var index = oldImg.lastIndexOf("/");
        var newImg = oldImg.slice(0, index + 1);
        newImg += img;
        if (oldImg != newImg) obj.src = newImg;
        if (obj.altbase) {
            obj.alt = obj.altbase;
        }
    }
}

function PresenceControlScroll() {
    if (!bPresenceControlInScrollFunc) {
        bPresenceControlInScrollFunc = true;
        PresenceControlHideOOUI();
    }
    bPresenceControlInScrollFunc = false;
    return PresenceControlOrigScrollFunc ? PresenceControlOrigScrollFunc() : true;
}


function PresenceControl(uri) {
    if (uri == null || uri == '') return;

    if (PresenceControlbrowseris.ie5up && PresenceControlbrowseris.win32) {
        var obj = window.event.srcElement;
        var objSpan = obj;
        var id = obj.id;
        var fFirst = false;
        if (!PresenceControlStatesDictionaryObj) {
            PresenceControlStatesDictionaryObj = new Object();
            PresenceControlURIDictionaryObj = new Object();
            if (!PresenceControlOrigScrollFunc) {
                PresenceControlOrigScrollFunc = window.onscroll;
                window.onscroll = PresenceControlScroll;
            }
        }
        if (PresenceControlStatesDictionaryObj) {
            if (!PresenceControlURIDictionaryObj[id]) {
                PresenceControlURIDictionaryObj[id] = uri;
                fFirst = true;
            }
            if (typeof (PresenceControlStatesDictionaryObj[id]) == "undefined") {
                PresenceControlStatesDictionaryObj[id] = 1;
            }
            if (fFirst && EnsurePresenceControl() && PresenceControlObject.PresenceEnabled) {
                var state = 1,
                    img;
                state = PresenceControlObject.GetStatus(uri, id);
                img = PresenceControlGetStatusImage(state);
                PresenceControlUpdateImage(id, img);
                PresenceControlStatesDictionaryObj[id] = state;
            }
        }
        if (fFirst) {
            var objRet = PresenceControlGetOOUILocation(obj, false);
            objSpan = objRet.objSpan;
            if (objSpan) {
                objSpan.onmouseover = PresenceControlShowOOUIMouse;
                objSpan.onfocusin = PresenceControlShowOOUIFocus;
                objSpan.onmouseout = PresenceControlHideOOUI;
                objSpan.onfocusout = PresenceControlHideOOUI;
            }
        }
    }
}

function PresenceControlHandleAccelerator() {
    if (PresenceControlObject) {
        if (event.altKey && event.shiftKey && event.keyCode == 121) {
            PresenceControlObject.DoAccelerator();
        }
    }
}

function PresenceControlGetOOUILocation(obj, fprint) {
    var objRet = new Object;
    var objSpan = obj;
    var objOOUI = obj;
    var oouiX = 0,
        oouiY = 0,
        objDX = 0;
    var fRtl = document.dir == "rtl";
    while (objSpan && objSpan.tagName != "SPAN" && objSpan.tagName != "TABLE") {
        objSpan = objSpan.parentNode;
    }
    if (objSpan) {
        var collNodes = objSpan.tagName == "TABLE" ? objSpan.rows(0).cells(0).childNodes : objSpan.childNodes;
        var i;
        for (i = 0; i < collNodes.length; ++i) {
            if (collNodes.item(i).tagName == "IMG" && collNodes.item(i).id) {
                objOOUI = collNodes.item(i);
                break;
            }
        }
    }
    obj = objOOUI;
    while (obj) {
        if (fRtl) {
            if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft) objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
            else objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
            oouiX += obj.offsetLeft + objDX;
        }
        else oouiX += obj.offsetLeft - obj.scrollLeft;
        oouiY += obj.offsetTop - obj.scrollTop;
        if (fprint) {
            alert(obj.scrollTop);
        }

        obj = obj.offsetParent;
    }
    try {
        obj = window.frameElement;
        while (obj) {
            if (fRtl) {
                if (obj.scrollWidth >= obj.clientWidth + obj.scrollLeft) objDX = obj.scrollWidth - obj.clientWidth - obj.scrollLeft;
                else objDX = obj.clientWidth + obj.scrollLeft - obj.scrollWidth;
                oouiX += obj.offsetLeft + objDX;
            }
            else oouiX += obj.offsetLeft - obj.scrollLeft;
            oouiY += obj.offsetTop - obj.scrollTop;
            if (fprint) {
                alert(obj.scrollTop);
            }

            obj = obj.offsetParent;
        }
    }
    catch (e)
    { };

    objRet.objSpan = objSpan;
    objRet.objOOUI = objOOUI;
    objRet.oouiX = oouiX;
    objRet.oouiY = oouiY;
    if (fRtl) objRet.oouiX += objOOUI.offsetWidth;

    if (fprint) {
        alert(oouiY);
    }

    return objRet;
}


function PresenceControlBrowseris() {
    var agt = navigator.userAgent.toLowerCase();
    this.osver = 1.0;
    if (agt) {
        var stOSVer = agt.substring(agt.indexOf("windows ") + 11);
        this.osver = parseFloat(stOSVer);
    }

    this.major = parseInt(navigator.appVersion);
    this.nav = ((agt.indexOf('mozilla') != -1) && ((agt.indexOf('spoofer') == -1) && (agt.indexOf('compatible') == -1)));
    this.nav2 = (this.nav && (this.major == 2));
    this.nav3 = (this.nav && (this.major == 3));
    this.nav4 = (this.nav && (this.major == 4));
    this.nav6 = this.nav && (this.major == 5);
    this.nav6up = this.nav && (this.major >= 5);
    this.nav7up = false;
    if (this.nav6up) {
        var navIdx = agt.indexOf("netscape/");
        if (navIdx >= 0) this.nav7up = parseInt(agt.substring(navIdx + 9)) >= 7;
    }
    this.ie = (agt.indexOf("msie") != -1);
    this.aol = this.ie && agt.indexOf(" aol ") != -1;
    if (this.ie) {
        var stIEVer = agt.substring(agt.indexOf("msie ") + 5);
        this.iever = parseInt(stIEVer);
        this.verIEFull = parseFloat(stIEVer);
    }
    else this.iever = 0;
    this.ie3 = (this.ie && (this.major == 2));
    this.ie4 = (this.ie && (this.major == 4));
    this.ie4up = this.ie && (this.major >= 4);
    this.ie5up = this.ie && (this.iever >= 5);
    this.ie55up = this.ie && (this.verIEFull >= 5.5);
    this.ie6up = this.ie && (this.iever >= 6);
    this.win16 = ((agt.indexOf("win16") != -1) || (agt.indexOf("16bit") != -1) || (agt.indexOf("windows 3.1") != -1) || (agt.indexOf("windows 16-bit") != -1));
    this.win31 = (agt.indexOf("windows 3.1") != -1) || (agt.indexOf("win16") != -1) || (agt.indexOf("windows 16-bit") != -1);
    this.win98 = ((agt.indexOf("win98") != -1) || (agt.indexOf("windows 98") != -1));
    this.win95 = ((agt.indexOf("win95") != -1) || (agt.indexOf("windows 95") != -1));
    this.winnt = ((agt.indexOf("winnt") != -1) || (agt.indexOf("windows nt") != -1));
    this.win32 = this.win95 || this.winnt || this.win98 || ((this.major >= 4) && (navigator.platform == "Win32")) || (agt.indexOf("win32") != -1) || (agt.indexOf("32bit") != -1);
    this.os2 = (agt.indexOf("os/2") != -1) || (navigator.appVersion.indexOf("OS/2") != -1) || (agt.indexOf("ibm-webexplorer") != -1);
    this.mac = (agt.indexOf("mac") != -1);
    this.mac68k = this.mac && ((agt.indexOf("68k") != -1) || (agt.indexOf("68000") != -1));
    this.macppc = this.mac && ((agt.indexOf("ppc") != -1) || (agt.indexOf("powerpc") != -1));
    this.w3c = this.nav6up;
}