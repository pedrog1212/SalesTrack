/** @type {import('tailwindcss').Config} */
// tailwind.config.js
const defaultTheme = require('tailwindcss/defaultTheme')
module.exports = {
  content: [
    './Pages/**/*.cshtml',        			// Razor Pages
    './Pages/Customers/Index.cshtml',        // Razor Pages
    './Pages/Orders/Index.cshtml',      	  // Razor Pages
    './Views/**/*.cshtml',        			// MVC Views (if any)
    './wwwroot/css/*.css',         			// Your CSS files (optional)
    './wwwroot/js/**/*.js'        			// Your JS files (optional)
  ],
  theme: {
    screens: {
      'xs': '361px',
      ...defaultTheme.screens,
    },
    extend: {
      colors: {
        'primary-color': '#0fa'
      }
    },
  },
  plugins: []
}
