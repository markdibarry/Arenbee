shader_type canvas_item;
render_mode blend_mix,unshaded;

uniform float _brightness : hint_range(-1, 1) = 0;
uniform float _contrast : hint_range(-1, 1) = 0;
uniform float _saturation : hint_range(-1, 2) = 0;
uniform vec4 _tint_color : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float _tint_amount : hint_range(0, 1) = 0;
uniform sampler2D mask_sampler : filter_nearest;
uniform vec3 _mask_transforms[3];

vec3 get_contrast(vec3 color, float contrast_amount) {
    return ((color - 0.5) * max(contrast_amount + 1.0, 0)) + 0.5;
}

vec3 get_brightness(vec3 color, float brightness_amount) {
    return color + brightness_amount;
}

vec3 get_saturation(vec3 color, float saturation_amount) {
    return mix(vec3(dot(vec3(1.0), color) * 0.33333), color, saturation_amount + 1.0);
}

vec3 get_tint(vec3 color, vec4 tint_color, float tint_amount) {
    vec3 c2 = color * tint_color.rgb;
    return mix(color, c2, tint_amount);
}

float get_mask_alpha(vec2 tex_size, vec2 tex_UV, vec2 mask_size, vec2 mask_position, float mask_scale) {
    vec2 mask_size_scaled = mask_size * mask_scale;
    vec2 mask_position_adj = (mask_position - mask_size_scaled * 0.5) / tex_size;
    vec2 mask_UV = (tex_UV - mask_position_adj) * (tex_size / mask_size_scaled);
    return texture(mask_sampler, mask_UV).a;
}

void fragment() {
    vec4 orig_color = texture(SCREEN_TEXTURE, SCREEN_UV);
    vec3 c = orig_color.rgb;
    c = get_tint(c, _tint_color, _tint_amount);
    c = get_contrast(c, _contrast);
    c = get_saturation(c, _saturation);
    c = get_brightness(c, _brightness);
    COLOR.rgb = c;
    vec2 texture_size = vec2(480,270);
    vec2 mask_size = vec2(textureSize(mask_sampler,0));
    float mask_alpha = 0.0;
    float size_mod = abs(mod(TIME * 5.0, 2.0) - 1.0);
    size_mod = size_mod * 0.05 + 0.975;
    //mask_alpha += get_mask_alpha(texture_size, UV, mask_size, _mask_transforms[0].xy, _mask_transforms[0].z);
    //mask_alpha = texture(mask_sampler, UV * (texture_size / mask_size)).a;
    for (int i = 0; i < 3; i++) {
        mask_alpha += get_mask_alpha(texture_size, UV, mask_size, _mask_transforms[i].xy, _mask_transforms[i].z * size_mod);
    }
    
    COLOR.a = clamp(orig_color.a - mask_alpha, 0.0, 1.0);
}