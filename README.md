# Real Estate Smart Prediction
A smart web application for predicting real estate prices using machine learning and user sentiment analysis.

## Project Overview
This system allows users to:
- View property listings
- Enter house features (area, rooms, location, etc.)
- Get predicted price using **Linear Regression**
- Leave feedback/comments
- Use **Sentiment Analysis** (positive/negative) on comments
- Adjust property rating based on comment tone

## Machine Learning
- **Linear Regression** to predict house prices based on numerical inputs.
- **Text Sentiment Analysis**:
  - Classify user comments as positive/negative
  - Uses NLP model (e.g., Naive Bayes / Logistic Regression / pretrained model)
  - Affects the final estate score or ranking

## Tech Stack
- Frontend: HTML, CSS, JavaScript
- Backend: ASP.NET MVC (.NET Framework)
- Database: Firebase
- ML: Python (trained models used in backend or via API)
