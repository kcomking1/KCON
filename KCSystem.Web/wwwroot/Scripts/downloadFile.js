function downloadBlob(url) {
    return new Promise((resolve, reject) => {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.responseType = "blob";

        xhr.onload = function () {
            resolve(xhr.response);
        };
        xhr.onerror = function () {
            reject(new Error("Download failed."));
        };
        xhr.send();
    });
}
function downloadFile(url, fileName = "") {
    return downloadBlob(url, fileName)
        .then(resp => {
            if (resp.blob) {
                return resp.blob();
            } else {
                return new Blob([resp]);
            }
        })
        .then(blob => URL.createObjectURL(blob))
        .then(url => {
            downloadURL(url, fileName);
            URL.revokeObjectURL(url);
        })
        .catch(err => {
            throw new Error(err.message);
        });
}

function downloadURL(url, name = "") {
    const link = document.createElement("a");
    link.download = name;
    link.href = url;
    if ("download" in document.createElement("a")) {
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } else {
        // 对不支持download进行兼容
        click(link, (link.target = "_blank"));
    }
}
// clone https://github.com/eligrey/FileSaver.js/blob/master/src/FileSaver.js
function click(node) {
    try {
        node.dispatchEvent(new MouseEvent("click"));
    } catch (e) {
        var evt = document.createEvent("MouseEvents");
        evt.initMouseEvent(
            "click",
            true,
            true,
            window,
            0,
            0,
            0,
            80,
            20,
            false,
            false,
            false,
            false,
            0,
            null
        );
        node.dispatchEvent(evt);
    }
}
