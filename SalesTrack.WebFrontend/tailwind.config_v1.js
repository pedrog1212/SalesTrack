/** @type {import('tailwindcss').Config} */
// tailwind.config.js
module.exports = {
  content: [
    './Pages/Customers/*.cshtml',        // Razor Pages
    './Pages/Orders/*.cshtml',        // Razor Pages
    './Views/**/*.cshtml',        // MVC Views (if any)
    './wwwroot/css/*.css'         // Your CSS files (optional)
 // './wwwroot/js/**/*.js'        // Your JS files (optional)
  ],
  theme: {
    extend: {}
  },
  plugins: []
}
