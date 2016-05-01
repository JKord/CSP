function sort(arr) {
    var len = arr.length;
    for (var i = len - 1; i >= 0; i--) {
        for (var j = 1; j <= i; j++) {
            if (arr[j - 1] > arr[j]) {
                var temp = arr[j - 1];
                arr[j - 1] = arr[j];
                arr[j] = temp;
            }
        }
    }

    return arr;
};

var items = [4,52,7,3,78,4];
var result = sort(array);

log('Sort array: ');
for (var i = 0; i < result.length; i++) {
    log(result[i] + ', ');
}

