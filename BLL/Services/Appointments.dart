// This is a simplified Dart version of Appointments.cshtml
// Original Purpose: Razor View for Displaying Appointments

import 'package:flutter/material.dart';

void main() => runApp(MyApp());

class MyApp extends StatelessWidget {
  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Appointments',
      home: Scaffold(
        appBar: AppBar(title: Text('Appointments Page')),
        body: Center(child: Text('This is a apointments page')),
      ),
    );
}
}
