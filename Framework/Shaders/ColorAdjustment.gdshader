shader_type canvas_item;
render_mode blend_mix,unshaded;

uniform float _brightness : hint_range(-1, 1) = 0;
uniform float _contrast : hint_range(0, 3) = 1.0;
uniform float _saturation : hint_range(0, 3) = 1.0;
uniform vec4 _tint_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float _tint_amount : hint_range(0, 1) = 0.0;

vec3 get_contrast(vec3 color, float contrast_amount) {
    return ((color - 0.5) * max(contrast_amount, 0)) + 0.5;
}

vec3 get_brightness(vec3 color, float brightness_amount) {
    return color + brightness_amount;
}

vec3 get_saturation(vec3 color, float saturation_amount){
    return mix(vec3(dot(vec3(1.0), color) * 0.33333), color, saturation_amount);
}

vec3 get_tint(vec3 color, vec4 tint_color, float tint_amount) {
    vec3 c2 = color * tint_color.rgb;
    return mix(color, c2, tint_amount);
}

void fragment() {
	vec3 c = texture(SCREEN_TEXTURE, SCREEN_UV).rgb;
    c = get_brightness(c, _brightness);
    c = get_contrast(c, _contrast);
    c = get_saturation(c, _saturation);
    c = get_tint(c, _tint_color, _tint_amount);
    COLOR.rgb = c;
}