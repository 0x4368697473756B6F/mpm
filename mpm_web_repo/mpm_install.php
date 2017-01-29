<?php
error_reporting(0);
$package_name = @$_GET['package'];
if ($package_name == null) {
	die(json_encode(['status' => 403, 'message' => 'Empty Package']));
}
$package_array = array(
	'Bamboo' => [
		'status' => 200,
		'name' => 'Bamboo',
		'version' => '1.7.10',
		'link' => 'http://127.0.0.1/mods/Bamboo/',
		'filename' => 'Bamboo.jar',
	],
	'CustomNPCs' => [
		'status' => 200,
		'name' => 'CustomNPCs',
		'version' => '1.7.10',
		'link' => 'http://127.0.0.1/mods/CustomNPCs/',
		'filename' => 'CustomNPCs.jar',
	],
	'FoodCraft' => [
		'status' => 200,
		'name' => 'FoodCraft',
		'version' => '1.7.10',
		'link' => 'http://127.0.0.1/mods/FoodCraft/',
		'filename' => 'FoodCraft.jar',
	],

);
if (isset($package_array[$package_name])) {
	echo json_encode($package_array[$package_name]);
} else {
	echo json_encode(['status' => 404, 'message' => 'Package Not Found']);
}