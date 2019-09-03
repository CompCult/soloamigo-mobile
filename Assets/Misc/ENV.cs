using System.Collections;
using System.Collections.Generic;

public static class ENV  {

// ACCESS
public static string
API_URL = "https://solo-amigo.herokuapp.com/",
GOOGLE_MAPS_KEY = "AIzaSyACtmxnxRymLJh3rhS0wkAaFsFgDNzXLDk",
GOOGLE_MAPS_URL = "https://maps.googleapis.com/maps/api/staticmap?center=PLACE&zoom=15&size=600x300&maptype=terrain&markers=color:green%7CPLACE",
GOOGLE_MAPS_COORD_URL = "https://maps.googleapis.com/maps/api/staticmap?center=PLACE&zoom=19&size=550x500&maptype=terrain&markers=color:green%7CPLACE";

// ROUTES
public static string
GENERAL_ROUTE = "general", 
USERS_ROUTE = "users",
PLANTS_ROUTE = "trees",
PLANTS_REQUEST_ROUTE= "tree_requests",
PLANTS_TYPES_ROUTE= "tree_types",
POSTS_ROUTE = "posts",
EVENTS_ROUTE = "appointment",
EVENTS_REQUESTS_ROUTE = "appointment_requests",
GROUPS_ROUTE = "groups",
GROUP_MEMBERS_ROUTE = "group_members",
QUIZZES_ROUTE = "quizzes",
QUIZ_ANSWERS_ROUTE = "quiz_answers",
MISSIONS_ROUTE = "missions",
MISSION_ANSWERS_ROUTE = "missions_answers";

// ACTIONS
public static string
MAINTENANCE_ACTION = "in_maintenance",
AUTH_ACTION = "auth",
EMAIL_ACTION = "email",
GROUPS_ACTION = "groups",
REGISTER_ACTION = "register",
UPDATE_ACTION = "update",
REMOVE_ACTION = "remove",
SEARCH_PUBLIC = "public?",
SEARCH_PRIVATE = "private?",
RECOVERY_ACTION = "recovery",
QUERY_ACTION = "query/fields?";

// MISC
public static string
FACEBOOK_PAGE = "https://www.facebook.com/pg/cineatos/",
INSTAGRAM_PAGE = "https://www.instagram.com/cineatos",
POINT = "Gemas",
GAME = "SoloAmigo";

}
