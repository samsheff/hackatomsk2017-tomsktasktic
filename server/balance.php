<?php
$url = file_get_contents('https://etherscan.io/address/0x12dcD003a65dea6370daa887439183E181891259');
function get_string_between($string, $start, $end){
    $string = ' ' . $string;
    $ini = strpos($string, $start);
    if ($ini == 0) return '';
    $ini += strlen($start);
    $len = strpos($string, $end, $ini) - $ini;
    return substr($string, $ini, $len);
}

$parsed = get_string_between($url, '<tr>
<td>ETH Balance:
</td>
<td>
', ' Ether');

$balance = str_replace('</b>','',str_replace('<b>', '', $parsed)) ;
echo $balance;