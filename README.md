 Alpha Parking SPA

 Project Description

This project is a simple parking management web application built using F# and WebSharper SPA architecture.

It simulates a real-world parking system where users can:
- park a car into a selected parking spot
- track parking duration in real time
- automatically calculate parking fees
- search parked cars by license plate
- process payments and free parking spots

The application is fully client-side and runs as a Single Page Application (SPA).


 Motivation

My personal motivation was that it came from the fact that I already had this idea in another subject that I wanted to implement in Python and I was interested in seeing if it would be possible to implement it in F#.

In addition, this project provided an opportunity to practice:

- functional programming in F#
- reactive UI development using WebSharper
- SPA architecture without a backend
- state management using reactive variables (`Var` / `View`)

The aim was to build a realistic small-scale system that demonstrates how functional programming can be used for interactive web applications.

 Features

- 10 parking spots (A1–A10)
- Real-time parking system (SPA)
- License plate registration
- Live duration tracking
- Dynamic price calculation based on time
- Payment system with confirmation popup
- Search functionality by license plate
- Sidebar navigation (Parking / Payment pages)
- Fully reactive UI updates

 Screenshots

Parking view:
screenshots/parking.png

Payment view:
screenshots/payment.png

Live Demo

https://torok-zeus.github.io/AlphaSPA/

Build & Run

Prerequisites:
- .NET SDK (8 or later)

Clone repository:
git clone https://github.com/torok-zeus/AlphaSPA.git
cd AlphaSPA

Build project:
dotnet build

Run:
This is a client-side SPA deployed via GitHub Pages.

Open:
https://torok-zeus.github.io/AlphaSPA/

 Notes

- No backend server is required
- The app runs entirely in the browser
- All state is stored in memory and resets on refresh
