export function numberToStringWithLength (toConvert: number, size: number): string {  
    var s = toConvert.toString();
    while (s.length < size)
       s = "0" + s;

    return s;
} 