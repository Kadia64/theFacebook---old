<?php 
$path = '/Projects/TheFacebook/Git/thefacebook/Server Functions/';
require_once $_SERVER['DOCUMENT_ROOT'] . $path . 'Scripts/content.php';
require_once $_SERVER['DOCUMENT_ROOT'] . $path . 'Scripts/data-handle.php';
require_once $_SERVER['DOCUMENT_ROOT'] . $path . 'Scripts/session-functions.php';
require_once $_SERVER['DOCUMENT_ROOT'] . $path . 'Scripts/sql-functions.php';
$content = new Content();
$dh = new DataHandle();
$sh = new SessionHandle();
$sql = new SQLHandle();
session_start();
$sql->Connect();

$email = $_POST['email'];
$username = $sql->GetUsernameByEmail($email);
$salt = $sql->GetValueByEmail('password_salt', 'account_info', $email);
$password = hash('sha256', $_POST['password'] . $salt);
$generated_sessionID = $dh->RandomCharacters(32);

if ($dh->Login($sql, $email, $password)) {
    $_SESSION['email'] = $email;
    $username = $sql->GetUsernameByEmail($email);
    $sh->StartuserSession($sql, $sql->GetIDByEmail($email), $generated_sessionID);
    $sh->SetUserTokenCookie($username, $email);
    $sql->CloseConnection();
    $sh->Redirect('Pages/User Pages/MainProfilePage.php?return-status=logged-in&username=' . $username . '&email=' . $email);
    exit;
} else {
    $sql->CloseConnection();
    $content->Alert('Username or password incorrect!');
    $sh->Redirect($_GET['prev-page'], 'js');
    exit;
}
?>