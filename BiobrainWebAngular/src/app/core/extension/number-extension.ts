interface Number {  
    toStringWithLength(size: number): String;  
}  

Number.prototype.toStringWithLength = function(size: number): string {  
    var s = Number(this).toString();
    while (s.length < size)
       s = "0" + s;

    return s;
} 